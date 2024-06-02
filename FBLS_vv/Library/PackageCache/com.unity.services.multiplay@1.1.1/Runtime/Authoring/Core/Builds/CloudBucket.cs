using System;

namespace Unity.Services.Multiplay.Authoring.Core.Builds
{
    struct CloudBucketId
    {
        public Guid Id { private get; init; }

        public Guid ToGuid() => Id;
        public override string ToString() => Id.ToString();
    }
}
