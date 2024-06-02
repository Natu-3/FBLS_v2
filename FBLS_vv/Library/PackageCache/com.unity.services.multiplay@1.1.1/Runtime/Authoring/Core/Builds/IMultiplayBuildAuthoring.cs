using System.Collections;
using System.Collections.Generic;

namespace Unity.Services.Multiplay.Authoring.Core
{
    internal interface IMultiplayBuildAuthoring
    {
        string BuildMultiplayServer(string directory, string executableName);

        void WarnBuildTargetChanged();
    }
}
