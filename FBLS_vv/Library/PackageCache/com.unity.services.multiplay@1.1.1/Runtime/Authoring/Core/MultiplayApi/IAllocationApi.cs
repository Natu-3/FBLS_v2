using System;
using System.Threading;
using System.Threading.Tasks;

namespace Unity.Services.Multiplay.Authoring.Core.MultiplayApi
{
    interface IAllocationApi
    {
        Task<AllocationResult> CreateTestAllocation(FleetId fleetId, Guid regionId, long buildConfigurationId, CancellationToken cancellationToken = default);
        Task<AllocationInformation> GetTestAllocation(FleetId fleetId, Guid allocationId, CancellationToken cancellationToken = default);
    }
}
