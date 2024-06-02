using System;

namespace Unity.Services.Multiplay.Authoring.Core.MultiplayApi
{
    struct FleetId
    {
        public Guid Id { private get; init; }

        public Guid ToGuid() => Id;
    }
}
