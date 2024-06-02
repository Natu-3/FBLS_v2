using Unity.Services.Multiplay.Authoring.Core.Builds;
using Unity.Services.Multiplay.Authoring.Core.Model;
using Unity.Services.Multiplay.Authoring.Core.MultiplayApi;

namespace Unity.Services.Multiplay.Authoring.Core.Deployment
{
    record BuildUploadResult(
        BuildItem BuildItem,
        BuildId BuildId,
        CloudBucketId CloudBucketId,
        int Changes);
}
