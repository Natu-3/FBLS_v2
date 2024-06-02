using System;
using Unity.Services.DeploymentApi.Editor;
using Unity.Services.Multiplay.Authoring.Core.Assets;

namespace Unity.Services.Multiplay.Authoring.Core.Model
{
    [Serializable]
    class BuildItem : DeploymentItem, IResourceRef
    {
        public BuildItem()
        {
            Type = "Build";
        }

        public BuildName OriginalName { get; set; }
        public MultiplayConfig.BuildDefinition Definition { get; set; }
        public IResourceName ResourceName => OriginalName;
    }
}
