using System;
using System.IO;
using UnityEditor.TestRunner.CommandLineParser;
using UnityEditor.TestTools.TestRunner.Api;

namespace UnityEditor.TestTools.TestRunner.CommandLineTest
{
    internal class SettingsBuilder : ISettingsBuilder
    {
        private ITestSettingsDeserializer m_TestSettingsDeserializer;
        private Action<string> m_LogAction;
        private Action<string> m_LogWarningAction;
        private Func<string, bool> m_FileExistsCheck;
        private Func<bool> m_ScriptCompilationFailedCheck;
        public SettingsBuilder(ITestSettingsDeserializer testSettingsDeserializer, Action<string> logAction, Action<string> logWarningAction, Func<string, bool> fileExistsCheck, Func<bool> scriptCompilationFailedCheck)
        {
            m_LogAction = logAction;
            m_LogWarningAction = logWarningAction;
            m_FileExistsCheck = fileExistsCheck;
            m_ScriptCompilationFailedCheck = scriptCompilationFailedCheck;
            m_TestSettingsDeserializer = testSettingsDeserializer;
        }

        public Api.ExecutionSettings BuildApiExecutionSettings(string[] commandLineArgs)
        {
            var quit = false;
            string testPlatform = TestMode.EditMode.ToString();
            string[] testFilters = null;
            string[] testCategories = null;
            string testSettingsFilePath = null;
            int? playerHeartbeatTimeout = null;
            bool runSynchronously = false;
            string[] testAssemblyNames = null;
<<<<<<<< Updated upstream:FBLS_vv/Library/PackageCache/com.unity.test-framework@1.1.29/UnityEditor.TestRunner/CommandLineTest/SettingsBuilder.cs
========
            string buildPlayerPath = string.Empty;
            string orderedTestListFilePath = null;
            int retry = 0;
            int repeat = 0;
            int randomOrderSeed = 0;

>>>>>>>> Stashed changes:FBLS_vv/Library/PackageCache/com.unity.test-framework/UnityEditor.TestRunner/CommandLineTest/SettingsBuilder.cs

            var optionSet = new CommandLineOptionSet(
                new CommandLineOption("quit", () => { quit = true; }),
                new CommandLineOption("testPlatform", platform => { testPlatform = platform; }),
                new CommandLineOption("editorTestsFilter", filters => { testFilters = filters; }),
                new CommandLineOption("testFilter", filters => { testFilters = filters; }),
                new CommandLineOption("editorTestsCategories", catagories => { testCategories = catagories; }),
                new CommandLineOption("testCategory", catagories => { testCategories = catagories; }),
                new CommandLineOption("testSettingsFile", settingsFilePath => { testSettingsFilePath = settingsFilePath; }),
                new CommandLineOption("playerHeartbeatTimeout", timeout => { playerHeartbeatTimeout = int.Parse(timeout); }),
                new CommandLineOption("runSynchronously", () => { runSynchronously = true; }),
<<<<<<<< Updated upstream:FBLS_vv/Library/PackageCache/com.unity.test-framework@1.1.29/UnityEditor.TestRunner/CommandLineTest/SettingsBuilder.cs
                new CommandLineOption("assemblyNames", assemblyNames => { testAssemblyNames = assemblyNames; })
========
                new CommandLineOption("assemblyNames", assemblyNames => { testAssemblyNames = assemblyNames; }),
                new CommandLineOption("buildPlayerPath", buildPath => { buildPlayerPath = buildPath; }),
                new CommandLineOption("orderedTestListFile", filePath => { orderedTestListFilePath = filePath; }),
                new CommandLineOption("randomOrderSeed", seed => { randomOrderSeed = int.Parse(seed);}),
                new CommandLineOption("retry", n => { retry = int.Parse(n); }),
                new CommandLineOption("repeat", n => { repeat = int.Parse(n); })
>>>>>>>> Stashed changes:FBLS_vv/Library/PackageCache/com.unity.test-framework/UnityEditor.TestRunner/CommandLineTest/SettingsBuilder.cs
            );
            optionSet.Parse(commandLineArgs);

            DisplayQuitWarningIfQuitIsGiven(quit);

            CheckForScriptCompilationErrors();

            var testSettings = GetTestSettings(testSettingsFilePath);
            var filter = new Filter
            {
                testMode = testPlatform.ToLower() == "editmode" ? TestMode.EditMode : TestMode.PlayMode,
                groupNames = testFilters,
                categoryNames = testCategories,
                assemblyNames = testAssemblyNames
            };

            var settings = new Api.ExecutionSettings
            {
                filters = new []{ filter },
                overloadTestRunSettings = new RunSettings(testSettings),
<<<<<<<< Updated upstream:FBLS_vv/Library/PackageCache/com.unity.test-framework@1.1.29/UnityEditor.TestRunner/CommandLineTest/SettingsBuilder.cs
                targetPlatform = buildTarget,
                runSynchronously = runSynchronously
========
                ignoreTests = testSettings?.ignoreTests,
                featureFlags = testSettings?.featureFlags,
                targetPlatform = GetBuildTarget(testPlatform),
                runSynchronously = runSynchronously,
                playerSavePath = buildPlayerPath,
                orderedTestNames = GetOrderedTestList(orderedTestListFilePath),
                repeatCount = repeat,
                retryCount = retry,
                randomOrderSeed = randomOrderSeed
>>>>>>>> Stashed changes:FBLS_vv/Library/PackageCache/com.unity.test-framework/UnityEditor.TestRunner/CommandLineTest/SettingsBuilder.cs
            };

            if (playerHeartbeatTimeout != null)
            {
                settings.playerHeartbeatTimeout = playerHeartbeatTimeout.Value;
            }

            return settings;
        }

        public ExecutionSettings BuildExecutionSettings(string[] commandLineArgs)
        {
            string resultFilePath = null;
            string deviceLogsDirectory = null;

            var optionSet = new CommandLineOptionSet(
                new CommandLineOption("editorTestsResultFile", filePath => { resultFilePath = filePath; }),
                new CommandLineOption("testResults", filePath => { resultFilePath = filePath; }),
                new CommandLineOption("deviceLogs", dirPath => { deviceLogsDirectory = dirPath; })
            );
            optionSet.Parse(commandLineArgs);

            return new ExecutionSettings
            {
                TestResultsFile = resultFilePath,
                DeviceLogsDirectory = deviceLogsDirectory
            };
        }

        private void DisplayQuitWarningIfQuitIsGiven(bool quitIsGiven)
        {
            if (quitIsGiven)
            {
                m_LogWarningAction("Running tests from command line arguments will not work when \"quit\" is specified.");
            }
        }

        private void CheckForScriptCompilationErrors()
        {
            if (m_ScriptCompilationFailedCheck())
            {
                throw new SetupException(SetupException.ExceptionType.ScriptCompilationFailed);
            }
        }

        private ITestSettings GetTestSettings(string testSettingsFilePath)
        {
            ITestSettings testSettings = null;
            if (!string.IsNullOrEmpty(testSettingsFilePath))
            {
                if (!m_FileExistsCheck(testSettingsFilePath))
                {
                    throw new SetupException(SetupException.ExceptionType.TestSettingsFileNotFound, testSettingsFilePath);
                }

                testSettings = m_TestSettingsDeserializer.GetSettingsFromJsonFile(testSettingsFilePath);
            }
            return testSettings;
        }
<<<<<<<< Updated upstream:FBLS_vv/Library/PackageCache/com.unity.test-framework@1.1.29/UnityEditor.TestRunner/CommandLineTest/SettingsBuilder.cs
========

        private string[] GetOrderedTestList(string orderedTestListFilePath)
        {
            if (!string.IsNullOrEmpty(orderedTestListFilePath))
            {
                if (!fileExistsCheck(orderedTestListFilePath))
                {
                    throw new SetupException(SetupException.ExceptionType.OrderedTestListFileNotFound, orderedTestListFilePath);
                }

                return readAllLines(orderedTestListFilePath);
            }
            return null;
        }
>>>>>>>> Stashed changes:FBLS_vv/Library/PackageCache/com.unity.test-framework/UnityEditor.TestRunner/CommandLineTest/SettingsBuilder.cs

        private static BuildTarget? GetBuildTarget(string testPlatform)
        {
            var testPlatformLower = testPlatform.ToLower();
            if (testPlatformLower == "editmode" || testPlatformLower == "playmode" || testPlatformLower == "editor" ||
                string.IsNullOrEmpty(testPlatformLower))
            {
                return null;
            }

            try
            {
                return (BuildTarget)Enum.Parse(typeof(BuildTarget), testPlatform, true);
            }
            catch (ArgumentException)
            {
                throw new SetupException(SetupException.ExceptionType.PlatformNotFound, testPlatform);
            }
        }
    }
}
