using System;
using System.Linq;
using UnityEditor.Compilation;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;
using UnityEngine.TestTools;

namespace UnityEditor.TestTools.TestRunner.UnityTestProtocol
{
    [InitializeOnLoad]
    internal static class UnityTestProtocolStarter
    {
        static UnityTestProtocolStarter()
        {
            var commandLineArgs = Environment.GetCommandLineArgs();
            //Ensuring that it is used only when tests are run using UTR.
            if (IsEnabled())
            {
                var api = ScriptableObject.CreateInstance<TestRunnerApi>();
                var listener = ScriptableObject.CreateInstance<UnityTestProtocolListener>();
                api.RegisterCallbacks(listener);
            }
        }
<<<<<<<< Updated upstream:FBLS_vv/Library/PackageCache/com.unity.test-framework@1.1.29/UnityEditor.TestRunner/UnityTestProtocol/UnityTestProtocolStarter.cs
========

        internal static bool IsEnabled()
        {
            var commandLineArgs = Environment.GetCommandLineArgs();
            return commandLineArgs.Contains("-automated") && commandLineArgs.Contains("-runTests");
        }

        private static string GetRepositoryPath(IReadOnlyList<string> commandLineArgs)
        {
            for (var i = 0; i < commandLineArgs.Count; i++)
            {
                if (commandLineArgs[i].Equals("-projectRepositoryPath"))
                {
                    return commandLineArgs[i + 1];
                }
            }
            return string.Empty;
        }
>>>>>>>> Stashed changes:FBLS_vv/Library/PackageCache/com.unity.test-framework/UnityEditor.TestRunner/UnityTestProtocol/UnityTestProtocolStarter.cs
    }
}
