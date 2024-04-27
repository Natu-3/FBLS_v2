using System;
using System.Collections.Generic;
using UnityEditor.TestTools.TestRunner.Api;
<<<<<<<< Updated upstream:FBLS_vv/Library/PackageCache/com.unity.test-framework@1.1.29/UnityEditor.TestRunner/TestRunner/Utils/CachingTestListProvider.cs
using UnityEngine.TestRunner.NUnitExtensions;
========
using UnityEditor.TestTools.TestRunner.Api.Analytics;
>>>>>>>> Stashed changes:FBLS_vv/Library/PackageCache/com.unity.test-framework/UnityEditor.TestRunner/TestRunner/Utils/CachingTestListProvider.cs
using UnityEngine.TestTools;

namespace UnityEditor.TestTools.TestRunner
{
    internal class CachingTestListProvider
    {
        private readonly ITestListProvider m_InnerTestListProvider;
        private readonly ITestListCache m_TestListCache;
        private readonly ITestAdaptorFactory m_TestAdaptorFactory;
        public CachingTestListProvider(ITestListProvider innerTestListProvider, ITestListCache testListCache, ITestAdaptorFactory testAdaptorFactory)
        {
            m_InnerTestListProvider = innerTestListProvider;
            m_TestListCache = testListCache;
            m_TestAdaptorFactory = testAdaptorFactory;
        }

        public IEnumerator<ITestAdaptor> GetTestListAsync(TestPlatform platform)
        {
            var testFromCache = m_TestListCache.GetTestFromCacheAsync(platform);
            while (testFromCache.MoveNext())
            {
                yield return null;
            }


            if (testFromCache.Current != null)
            {
                yield return testFromCache.Current;
            }
            else
            {
                var test = m_InnerTestListProvider.GetTestListAsync(platform);
                while (test.MoveNext())
                {
                    yield return null;
                }

                test.Current.ParseForNameDuplicates();
                m_TestListCache.CacheTest(platform, test.Current);
<<<<<<<< Updated upstream:FBLS_vv/Library/PackageCache/com.unity.test-framework@1.1.29/UnityEditor.TestRunner/TestRunner/Utils/CachingTestListProvider.cs
========
#if !UNITY_2023_2_OR_NEWER
                AnalyticsReporter.AnalyzeTestTreeAndReport(test.Current);
#endif
>>>>>>>> Stashed changes:FBLS_vv/Library/PackageCache/com.unity.test-framework/UnityEditor.TestRunner/TestRunner/Utils/CachingTestListProvider.cs
                yield return m_TestAdaptorFactory.Create(test.Current);
            }
        }
    }
}
