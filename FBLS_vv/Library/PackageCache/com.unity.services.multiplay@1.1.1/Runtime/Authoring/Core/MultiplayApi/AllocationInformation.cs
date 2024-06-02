using System;

namespace Unity.Services.Multiplay.Authoring.Core.MultiplayApi
{
    record AllocationInformation(Guid AllocationId, Guid FleetId, Guid RegionId, long BuildConfigurationId,
        long ServerId, long MachineId, string Ipv4Address, string Ipv6Address, long GamePort);
}
