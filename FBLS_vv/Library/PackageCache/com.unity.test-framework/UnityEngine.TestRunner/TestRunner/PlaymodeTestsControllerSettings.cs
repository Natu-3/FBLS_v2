using System;
using System.Linq;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal.Filters;
using UnityEngine.SceneManagement;
using UnityEngine.TestRunner.NUnitExtensions.Runner;
using UnityEngine.TestTools.TestRunner.GUI;

namespace UnityEngine.TestTools.TestRunner
{
    [Serializable]
    internal class PlaymodeTestsControllerSettings
    {
        [SerializeField]
        public RuntimeTestRunnerFilter[] filters;
        public bool sceneBased;
        public string originalScene;
        public string bootstrapScene;
<<<<<<<< Updated upstream:FBLS_vv/Library/PackageCache/com.unity.test-framework@1.1.29/UnityEngine.TestRunner/TestRunner/PlaymodeTestsControllerSettings.cs

        public static PlaymodeTestsControllerSettings CreateRunnerSettings(RuntimeTestRunnerFilter[] filters)
========
        public bool runInBackgroundValue;
        public bool consoleErrorPaused;
        public string[] orderedTestNames;
        public FeatureFlags featureFlags;
        [SerializeField]
        public int retryCount;

        [SerializeField]
        public int repeatCount;

        [SerializeField]
        public bool automated;

        [SerializeField]
        public int randomOrderSeed;

        public static PlaymodeTestsControllerSettings CreateRunnerSettings(RuntimeTestRunnerFilter[] filters, string[] orderedTestNames, int randomSeed, FeatureFlags featureFlags, int retryCount, int repeatCount, bool automated)
>>>>>>>> Stashed changes:FBLS_vv/Library/PackageCache/com.unity.test-framework/UnityEngine.TestRunner/TestRunner/PlaymodeTestsControllerSettings.cs
        {
            var settings = new PlaymodeTestsControllerSettings
            {
                filters = filters,
                sceneBased = false,
                originalScene = SceneManager.GetActiveScene().path,
<<<<<<<< Updated upstream:FBLS_vv/Library/PackageCache/com.unity.test-framework@1.1.29/UnityEngine.TestRunner/TestRunner/PlaymodeTestsControllerSettings.cs
                bootstrapScene = null
========
                bootstrapScene = null,
                orderedTestNames = orderedTestNames,
                randomOrderSeed = randomSeed,
                featureFlags = featureFlags,
                retryCount = retryCount,
                repeatCount = repeatCount,
                automated = automated
>>>>>>>> Stashed changes:FBLS_vv/Library/PackageCache/com.unity.test-framework/UnityEngine.TestRunner/TestRunner/PlaymodeTestsControllerSettings.cs
            };
            return settings;
        }

        internal ITestFilter BuildNUnitFilter()
        {
            return new OrFilter(filters.Select(f => f.BuildNUnitFilter()).ToArray());
        }
    }
}
