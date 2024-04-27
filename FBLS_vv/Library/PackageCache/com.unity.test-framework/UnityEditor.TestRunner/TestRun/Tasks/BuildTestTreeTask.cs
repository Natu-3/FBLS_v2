using System;
using System.Collections;
using System.Linq;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine.TestTools;
using UnityEngine.TestTools.NUnitExtensions;

namespace UnityEditor.TestTools.TestRunner.TestRun.Tasks
{
    internal class BuildTestTreeTask : TestTaskBase
    {
        private TestPlatform m_TestPlatform;

        public BuildTestTreeTask(TestPlatform testPlatform)
        {
            m_TestPlatform = testPlatform;
            RerunAfterResume = true;
        }

        internal IEditorLoadedTestAssemblyProvider m_testAssemblyProvider = new EditorLoadedTestAssemblyProvider(new EditorCompilationInterfaceProxy(), new EditorAssembliesProxy());
<<<<<<<< Updated upstream:FBLS_vv/Library/PackageCache/com.unity.test-framework@1.1.29/UnityEditor.TestRunner/TestRun/Tasks/BuildTestTreeTask.cs
        internal IAsyncTestAssemblyBuilder m_testAssemblyBuilder = new UnityTestAssemblyBuilder();
========
        internal Func<string[], int, IAsyncTestAssemblyBuilder> m_testAssemblyBuilderFactory = (orderedTestNames, seed) => new UnityTestAssemblyBuilder(orderedTestNames, seed);
>>>>>>>> Stashed changes:FBLS_vv/Library/PackageCache/com.unity.test-framework/UnityEditor.TestRunner/TestRun/Tasks/BuildTestTreeTask.cs
        internal ICallbacksDelegator m_CallbacksDelegator = CallbacksDelegator.instance;

        public override IEnumerator Execute(TestJobData testJobData)
        {
            if (testJobData.testTree != null)
            {
                yield break;
            }
            
            var assembliesEnumerator = m_testAssemblyProvider.GetAssembliesGroupedByTypeAsync(m_TestPlatform);
            while (assembliesEnumerator.MoveNext())
            {
                yield return null;
            }

            if (assembliesEnumerator.Current == null)
            {
                throw new Exception("Assemblies not retrieved.");
            }
            
            var assemblies = assembliesEnumerator.Current.Where(pair => m_TestPlatform.IsFlagIncluded(pair.Key)).SelectMany(pair => pair.Value).Select(x => x.Assembly).ToArray();
            var buildSettings = UnityTestAssemblyBuilder.GetNUnitTestBuilderSettings(m_TestPlatform);
<<<<<<<< Updated upstream:FBLS_vv/Library/PackageCache/com.unity.test-framework@1.1.29/UnityEditor.TestRunner/TestRun/Tasks/BuildTestTreeTask.cs
            var enumerator = m_testAssemblyBuilder.BuildAsync(assemblies, Enumerable.Repeat(m_TestPlatform, assemblies.Length).ToArray(), buildSettings);
========
            var testAssemblyBuilder = m_testAssemblyBuilderFactory(testJobData.executionSettings.orderedTestNames, testJobData.executionSettings.randomOrderSeed);
            var enumerator = testAssemblyBuilder.BuildAsync(assemblies, Enumerable.Repeat(m_TestPlatform, assemblies.Length).ToArray(), buildSettings);
>>>>>>>> Stashed changes:FBLS_vv/Library/PackageCache/com.unity.test-framework/UnityEditor.TestRunner/TestRun/Tasks/BuildTestTreeTask.cs
            while (enumerator.MoveNext())
            {
                yield return null;
            }

            var testList = enumerator.Current;
            if (testList== null)
            {
                throw new Exception("Test list not retrieved.");
            }
<<<<<<<< Updated upstream:FBLS_vv/Library/PackageCache/com.unity.test-framework@1.1.29/UnityEditor.TestRunner/TestRun/Tasks/BuildTestTreeTask.cs
            
            testList.ParseForNameDuplicates();
========

>>>>>>>> Stashed changes:FBLS_vv/Library/PackageCache/com.unity.test-framework/UnityEditor.TestRunner/TestRun/Tasks/BuildTestTreeTask.cs
            testJobData.testTree = testList;
            m_CallbacksDelegator.TestTreeRebuild(testList);
        }
    }
}
