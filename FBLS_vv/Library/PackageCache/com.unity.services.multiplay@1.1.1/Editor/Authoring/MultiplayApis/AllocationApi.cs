using System;
using System.Threading;
using System.Threading.Tasks;
using Unity.Services.Multiplay.Authoring.Core;
using Unity.Services.Multiplay.Authoring.Core.MultiplayApi;
using Unity.Services.Multiplay.Authoring.Editor.AdminApis.Allocations.Allocations;
using Unity.Services.Multiplay.Authoring.Editor.AdminApis.Allocations.Apis.Allocations;
using Unity.Services.Multiplay.Authoring.Editor.AdminApis.Allocations.Models;

namespace Unity.Services.Multiplay.Authoring.Editor.MultiplayApis
{
    class AllocationApi : IAllocationApi
    {
        readonly IAllocationsApiClient m_Client;
        readonly IMultiplayApiConfig m_ApiConfig;

        public AllocationApi(IMultiplayApiConfig apiConfig, IAllocationsApiClient client)
        {
            m_ApiConfig = apiConfig;
            m_Client = client;
        }

        public async Task<AllocationResult> CreateTestAllocation(FleetId fleetId, Guid regionId, long buildConfigurationId, CancellationToken cancellationToken = default)
        {
            var emptyGuid = Guid.NewGuid();
            var form = new TestAllocateRequestForm(regionId, buildConfigurationId, emptyGuid);

            var request = new ProcessTestAllocationRequest(m_ApiConfig.ProjectId, m_ApiConfig.EnvironmentId, fleetId.ToGuid(), form);
            var response = await TryCatchRequestAsync(request, async(req) => {
                return await m_Client.ProcessTestAllocationAsync(request, null);
            });
            return new AllocationResult(response.Result.AllocationId, response.Result.Href);
        }

        public async Task<AllocationInformation> GetTestAllocation(FleetId fleetId, Guid allocationId,
            CancellationToken cancellationToken = default)
        {
            var request = new GetTestAllocationRequest(m_ApiConfig.ProjectId, m_ApiConfig.EnvironmentId, fleetId.ToGuid(), allocationId);
            var response = await TryCatchRequestAsync(request, async(req) => {
                return await m_Client.GetTestAllocationAsync(request, null);
            });

            if (response.Status != 200 || response.Result.ServerId == 0)
            {
                return null;
            }
            return new AllocationInformation(
                response.Result.AllocationId,
                response.Result.FleetId,
                response.Result.RegionId,
                response.Result.BuildConfigurationId,
                response.Result.ServerId,
                response.Result.MachineId,
                response.Result.Ipv4,
                response.Result.Ipv6,
                response.Result.GamePort);
        }

        async Task<AdminApis.Allocations.Response<TResponse>> TryCatchRequestAsync<TRequest, TResponse>(TRequest request, Func<TRequest, Task<AdminApis.Allocations.Response<TResponse>>> func)
        {
            AdminApis.Allocations.Response<TResponse> response;
            try
            {
                response = await func(request);
            }
            catch (AdminApis.Allocations.Http.HttpException<ListTestAllocations400Response> ex)
            {
                throw new MultiplayAuthoringException((int)ex.Response.StatusCode, ex.ActualError.Detail, ex);
            }
            catch (AdminApis.Allocations.Http.HttpException<ListTestAllocations401Response> ex)
            {
                throw new MultiplayAuthoringException((int)ex.Response.StatusCode, ex.ActualError.Detail, ex);
            }
            catch (AdminApis.Allocations.Http.HttpException<ListTestAllocations403Response> ex)
            {
                throw new MultiplayAuthoringException((int)ex.Response.StatusCode, ex.ActualError.Detail, ex);
            }
            catch (AdminApis.Allocations.Http.HttpException<ListTestAllocations404Response> ex)
            {
                throw new MultiplayAuthoringException((int)ex.Response.StatusCode, ex.ActualError.Detail, ex);
            }
            catch (AdminApis.Allocations.Http.HttpException<ListTestAllocations429Response> ex)
            {
                throw new MultiplayAuthoringException((int)ex.Response.StatusCode, ex.ActualError.Detail, ex);
            }
            catch (AdminApis.Allocations.Http.HttpException<ListTestAllocations500Response> ex)
            {
                throw new MultiplayAuthoringException((int)ex.Response.StatusCode, "Internal Server Error", ex);
            }
            return response;
        }
    }
}
