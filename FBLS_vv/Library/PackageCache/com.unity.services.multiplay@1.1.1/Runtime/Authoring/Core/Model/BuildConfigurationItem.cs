using System;
using Unity.Services.DeploymentApi.Editor;
using Unity.Services.Multiplay.Authoring.Core.Assets;

namespace Unity.Services.Multiplay.Authoring.Core.Model
{
    [Serializable]
    class BuildConfigurationItem : DeploymentItem, IResourceRef
    {
        public BuildConfigurationItem()
        {
            Type = "Build Configuration";
        }

        public BuildConfigurationName OriginalName { get; set; }
        public MultiplayConfig.BuildConfigurationDefinition Definition { get; set; }
        public IResourceName ResourceName => OriginalName;
    }
}
