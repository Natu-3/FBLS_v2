using System;

namespace UnityEditor.TestTools.TestRunner.UnityTestProtocol
{
    internal class TestFinishedMessage : Message
    {
        public string name;
        public TestState state;
        public string message;
        public ulong duration; // milliseconds
        public ulong durationMicroseconds;
        public string stackTrace;
<<<<<<<< Updated upstream:FBLS_vv/Library/PackageCache/com.unity.test-framework@1.1.29/UnityEditor.TestRunner/UnityTestProtocol/TestFinishedMessage.cs
========
        public string fileName;
        public int lineNumber;
        public int iteration;
>>>>>>>> Stashed changes:FBLS_vv/Library/PackageCache/com.unity.test-framework/UnityEditor.TestRunner/UnityTestProtocol/Messages/TestFinishedMessage.cs

        public TestFinishedMessage()
        {
            type = "TestStatus";
            phase = "End";
        }
    }
}
