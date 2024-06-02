using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

namespace Testing
{
    /// <summary>
    /// Messaging reciever and callback handler for communication when doing remote
    /// testing via a test orchestrator client.
    /// 
    /// RPC callable functions use RemoteTestFixtureAttribute to declare accessibility.
    /// </summary>
    public class TestAgent
    {
        //Pairwise enum is maintained with Client (Test Orchestrator), values should be identical
        enum RemoteTestStatus
        {
            None = 0,
            Passed,
            Failed,
            InvalidRpc,
            Timeout             // <-- TODO as current API only provides for synchronous calls
        }

        const string getExceptionMessage = "!e";
        const string quitMessage = "!q";

        private Exception mostRecentException;
        private IMessageSource messenger;
        private bool isBusy = false;

        public event Action OnQuitMessageRecieved;

        //Custom message handlers with callbacks can be registered here
        private Dictionary<string, Action> customHandlers = new Dictionary<string, Action>();

        public TestAgent()
        {
        }

        public void RegisterMessenger(IMessageSource source) 
        {
            source.OnMessageRecieved += OnMessageRecieved;
            messenger = source;
        }

        public void UnregisterMessenger(IMessageSource source) 
        {
            source.OnMessageRecieved -= OnMessageRecieved;
            messenger = null;
        }

        public void AddCustomMessageHandler(string message, Action handler) 
        {
            if (message == quitMessage || message == getExceptionMessage)
            {
                Debug.LogError($"[{nameof(TestAgent)}]: '{quitMessage}' and '{getExceptionMessage}' are reserved for internal use! Custom message assignment ignored.");
                return;
            }
            else if (customHandlers.ContainsKey(message)) 
            {
                Debug.LogWarning($"[{nameof(TestAgent)}]: A custom handler for message '{message}' has already been registered! Ignoring..");
                return;
            }

            customHandlers.Add(message, handler);
        }

        public string GetExceptionMessage() 
        {
            if (mostRecentException == null) 
            {
                Debug.LogWarning($"[{nameof(TestAgent)}]: Attempted to get most recent exception when one wasn't thrown!");
                return null;
            }

            //Try and get the first inner exception that is not a aggregate or targetinvocation exception 
            // - this will help get a more useful message besides 'exception was thrown'.
            var exception = mostRecentException;
            while (exception.GetType() == typeof(AggregateException) || exception.GetType() == typeof(TargetInvocationException)) 
            {
                if (exception.InnerException == null) 
                {
                    break;
                }

                exception = exception.InnerException;
            }

            return $"{exception.GetType().Name}: '{exception.Message}'";
        }

        private async Task OnMessageRecieved(string message)
        {
            if (isBusy) 
            {
                Debug.LogWarning($"[{nameof(TestAgent)}]: Recieved message '{message}' but was busy running other operations...");
                return;
            }

            if (message.Length == 0) 
            {
                return;
            }

            isBusy = true;
            Debug.Log($"[{nameof(TestAgent)}]: Message recieved - '{message}'");

            if (message == getExceptionMessage)
            {
                if (mostRecentException != null)
                {
                    Debug.Log($"[{nameof(TestAgent)}]: Logging out most recently encountered exception");
                    Debug.LogException(mostRecentException);
                    messenger.Send(GetExceptionMessage());
                }
                isBusy = false;
                return;
            }
            else if (message == quitMessage) 
            {
                if (OnQuitMessageRecieved == null)
                {
                    Debug.LogWarning($"[{nameof(TestAgent)}]: Quit message was received but no function to handle quit operation was assigned to the 'OnQuitMessageRecieved' event!");
                    isBusy = false;
                    return;
                }
                else 
                {
                    OnQuitMessageRecieved.Invoke();
                }
            }
            else if (customHandlers.ContainsKey(message))
            {
                customHandlers[message]?.Invoke();
            }
            else
            {
                await RunTestHandler(message);
            }
            isBusy = false;
        }

        private async Task RunTestHandler(string message) 
        {
            RemoteTestStatus status = RemoteTestStatus.None;
            try
            {
                var split = message.Split('.');
                if (split.Length <= 1)
                {
                    string errorMessage = $"[{nameof(TestAgent)}]: Expected message should be of the format 'Namespace.Classname.FunctionName'. Recieved: {message}";
                    Debug.LogError(errorMessage);
                    mostRecentException = new InvalidOperationException(errorMessage);
                    status = RemoteTestStatus.InvalidRpc;
                    return;
                }

                var classFullNameSplit = split.Take(split.Count() - 1).ToArray();
                Type fixtureType = Type.GetType(string.Join(".", classFullNameSplit), true, true);
                status = await RunTest(fixtureType, split[split.Length-1]);
            }
            catch (Exception e)
            {
                mostRecentException = e;

                //Types of exception that can be thrown by Type.GetType(String, Boolean, Boolean)
                if (e is ArgumentNullException 
                    || e is ArgumentException 
                    || e is TypeLoadException 
                    || e is FileNotFoundException 
                    || e is FileLoadException
                    || e is BadImageFormatException)
                {
                    status = RemoteTestStatus.InvalidRpc;
                }
                else 
                {
                    status = RemoteTestStatus.Failed;
                }
            }
            finally
            {
                //Return the result to the callee
                Debug.Log($"[{nameof(TestAgent)}]: Sending '{status}'");
                messenger.Send(status.ToString());
            }
        }

        private async Task<RemoteTestStatus> RunTest(Type type, string functionName)
        {
            MethodInfo functionToExecute = type.GetMethod(functionName);

            //Check function exists (classname needs to exist in the executing scope)
            if (functionToExecute == null)
            {
                var message = $"[{nameof(TestAgent)}]: Found classtype '{type.Name}' did not have a method signature matching '{functionName}'!";
                Debug.LogError(message);
                mostRecentException = new InvalidOperationException(message);
                return RemoteTestStatus.InvalidRpc;
            }

            //Check this class supports RPC calls
            bool isTestFixture = type.IsDefined(typeof(RemoteTestFixtureAttribute));
            if (!isTestFixture)
            {
                var message = $"[{nameof(TestAgent)}]: The targeted class was not declared as available to be run remotely!";
                Debug.LogError(message);
                mostRecentException = new InvalidOperationException(message);
                return RemoteTestStatus.InvalidRpc;
            }

            Debug.Log($"[{nameof(TestAgent)}]: Running test - {type.Name}:{functionName}...");
            await InvokeTarget(type, functionToExecute);

            //If we completed all previous calls without exceptions, we can assume test passed.
            //TODO - address timeouts when implemented
            return RemoteTestStatus.Passed;
        }

        private async Task InvokeTarget(Type targetType, MethodInfo method)
        {
            object target = (method.IsStatic) ? null : Activator.CreateInstance(targetType);
            var isAwaitable = method.ReturnType.GetMethod(nameof(Task.GetAwaiter)) != null;
            object[] emptyArgs = new object[0];

            //N.B. No argument passing to test fixture included in scope of implementation
            if (isAwaitable)
            {
                await (Task)method.Invoke(target, emptyArgs);
            }
            else
            {
                //We don't care for the return value of a fixture
                method.Invoke(target, emptyArgs);
            }
        }
    }

    /// <summary>
    /// Decorate a non-static class with this to declare all it's methods available to be called remotely.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class RemoteTestFixtureAttribute : Attribute
    { 
    }
}
