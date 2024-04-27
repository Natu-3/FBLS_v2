using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NUnit;
using NUnit.Framework.Api;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
<<<<<<<< Updated upstream:FBLS_vv/Library/PackageCache/com.unity.test-framework@1.1.29/UnityEngine.TestRunner/NUnitExtensions/UnityTestAssemblyBuilder.cs
========
using Unity.Profiling;
using UnityEngine.TestRunner.NUnitExtensions;
using UnityEngine.TestRunner.NUnitExtensions.Filters;
>>>>>>>> Stashed changes:FBLS_vv/Library/PackageCache/com.unity.test-framework/UnityEngine.TestRunner/NUnitExtensions/UnityTestAssemblyBuilder.cs

namespace UnityEngine.TestTools.NUnitExtensions
{
    internal class UnityTestAssemblyBuilder : DefaultTestAssemblyBuilder, IAsyncTestAssemblyBuilder
    {
        private readonly string m_ProductName;
<<<<<<<< Updated upstream:FBLS_vv/Library/PackageCache/com.unity.test-framework@1.1.29/UnityEngine.TestRunner/NUnitExtensions/UnityTestAssemblyBuilder.cs
        public UnityTestAssemblyBuilder()
        {
========
        private readonly ITestSuiteModifier[] m_TestSuiteModifiers;

        public UnityTestAssemblyBuilder(string[] orderedTestNames, int randomSeed)
        {
            m_TestSuiteModifiers = (orderedTestNames != null && orderedTestNames.Length > 0) || randomSeed != 0
                ? new ITestSuiteModifier[] {new OrderedTestSuiteModifier(orderedTestNames, randomSeed)}
                : new ITestSuiteModifier[0];
>>>>>>>> Stashed changes:FBLS_vv/Library/PackageCache/com.unity.test-framework/UnityEngine.TestRunner/NUnitExtensions/UnityTestAssemblyBuilder.cs
            m_ProductName = Application.productName;
        }

        public ITest Build(Assembly[] assemblies, TestPlatform[] testPlatforms, IDictionary<string, object> options)
        {
            var test = BuildAsync(assemblies, testPlatforms, options);
            while (test.MoveNext())
            {
            }

            return test.Current;
        }

        public IEnumerator<ITest> BuildAsync(Assembly[] assemblies, TestPlatform[] testPlatforms, IDictionary<string, object> options)
        {
            var productName = string.Join("_", m_ProductName.Split(Path.GetInvalidFileNameChars()));
            var suite = new TestSuite(productName);
            for (var index = 0; index < assemblies.Length; index++)
            {
                var assembly = assemblies[index];
                var platform = testPlatforms[index];

                using (new ProfilerMarker(nameof(UnityTestAssemblyBuilder) + "." + assembly.GetName().Name).Auto())
                {
                    var assemblySuite = Build(assembly, GetNUnitTestBuilderSettings(platform)) as TestSuite;
                    if (assemblySuite != null)
                    {
                        assemblySuite.Properties.Set("platform", platform);
                        assemblySuite.Properties.Set("isAssembly", true);
                        EditorOnlyFilter.ApplyPropertyToTest(assemblySuite, platform == TestPlatform.EditMode);
                    }

                    if (assemblySuite != null && assemblySuite.HasChildren)
                    {
                        suite.Add(assemblySuite);
                    }
                }

                yield return null;
            }
<<<<<<<< Updated upstream:FBLS_vv/Library/PackageCache/com.unity.test-framework@1.1.29/UnityEngine.TestRunner/NUnitExtensions/UnityTestAssemblyBuilder.cs
========

            suite.ParseForNameDuplicates();
            suite.Properties.Set("platform", testPlatforms.MergeFlags());

            foreach (var testSuiteModifier in m_TestSuiteModifiers)
            {
                suite = testSuiteModifier.ModifySuite(suite);
            }
>>>>>>>> Stashed changes:FBLS_vv/Library/PackageCache/com.unity.test-framework/UnityEngine.TestRunner/NUnitExtensions/UnityTestAssemblyBuilder.cs

            yield return suite;
        }

        public static Dictionary<string, object> GetNUnitTestBuilderSettings(TestPlatform testPlatform)
        {
            var emptySettings = new Dictionary<string, object>();
            emptySettings.Add(FrameworkPackageSettings.TestParameters, "platform=" + testPlatform);
            return emptySettings;
        }
    }
}
