using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.Services.Multiplay.Authoring.Core;
using Unity.Services.Multiplay.Authoring.Core.Assets;
using Unity.Services.Multiplay.Authoring.Core.MultiplayApi;
using Unity.Services.Multiplay.Authoring.Editor.AdminApis.Fleets.Apis.Fleets;
using Unity.Services.Multiplay.Authoring.Editor.AdminApis.Fleets.Fleets;
using Unity.Services.Multiplay.Authoring.Editor.AdminApis.Fleets.Models;

namespace Unity.Services.Multiplay.Authoring.Editor.MultiplayApis
{
    class FleetApi : IFleetApi
    {
        readonly IMultiplayApiConfig m_ApiConfig;
        readonly IFleetsApiClient m_Client;

        public FleetApi(IMultiplayApiConfig apiConfig, IFleetsApiClient client)
        {
            m_ApiConfig = apiConfig;
            m_Client = client;
        }

        public async Task<IReadOnlyList<FleetInfo>> List(CancellationToken cancellationToken = default)
        {
            var request = new ListFleetsRequest(m_ApiConfig.ProjectId, m_ApiConfig.EnvironmentId);
            var response = await TryCatchRequestAsync(request, async(req) => {
                return await m_Client.ListFleetsAsync(req);
            });

            var responseList = response.Result;
            var res = new List<FleetInfo>();

            foreach (var resItem in responseList)
            {
                res.Add(new FleetInfo(
                    resItem.Name,
                    id: new FleetId { Id = resItem.Id },
                    fleetStatus: FromApi(resItem.Status, resItem.Name),
                    osId: resItem.OsID,
                    osName: resItem.OsName,
                    regions: FromApi(resItem.Regions),
                    allocationStatus: FromApi(resItem.Servers)
                ));
            }

            return res;
        }

        public async Task<FleetInfo> FindByName(string name, CancellationToken cancellationToken = default)
        {
            var request = new ListFleetsRequest(m_ApiConfig.ProjectId, m_ApiConfig.EnvironmentId);
            var response = await TryCatchRequestAsync(request, async(req) => {
                return await m_Client.ListFleetsAsync(req);
            });

            var result = response.Result.FirstOrDefault(
                b => b.Name == name);

            if (result == null)
                return null;

            return new FleetInfo(
                result.Name,
                new FleetId
                { Id = result.Id} ,
                FromApi(result.Status, name),
                result.OsID,
                result.OsName,
                FromApi(result.Regions));
        }

        static FleetInfo.Status FromApi(FleetListItem.StatusOptions statusOption, string fleetName)
        {
            switch (statusOption)
            {
                case FleetListItem.StatusOptions.ONLINE:
                    return FleetInfo.Status.Online;
                case FleetListItem.StatusOptions.DRAINING:
                    return FleetInfo.Status.Draining;
                case FleetListItem.StatusOptions.OFFLINE:
                    return FleetInfo.Status.Offline;
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(statusOption),
                        statusOption,
                        $"Unrecognized remote fleet status '{statusOption}' from fleet '{fleetName}'");
            }
        }

        static List<FleetInfo.FleetRegionInfo> FromApi(List<FleetRegion> regions)
        {
            return regions.Select(r => new FleetInfo.FleetRegionInfo(r.RegionID, r.RegionID, r.RegionName)).ToList();
        }

        public async Task<FleetInfo> Create(string name, IList<BuildConfigurationId> buildConfigurations, MultiplayConfig.FleetDefinition definition, CancellationToken cancellationToken = default)
        {
            var regions = await GetRegions();

            var fleet = new FleetCreateRequest(
                name,
                buildConfigurations.Select(b => b.ToLong()).ToList(),
                definition.Regions.Select(r => new Region(regions[r.Key], r.Value.MinAvailable, r.Value.MaxServers)).ToList(),
                osID: Guid.Empty, // Must be set in order to avoid breaking the API
                osFamily: FleetCreateRequest.OsFamilyOptions.LINUX);
            var request = new CreateFleetRequest(m_ApiConfig.ProjectId, m_ApiConfig.EnvironmentId, fleet);
            var response = await TryCatchRequestAsync(request, async(req) => {
                return await m_Client.CreateFleetAsync(req);
            });
            return new FleetInfo(response.Result.Name, new FleetId { Id = response.Result.Id }, FromApi(response.Result.Status, name), response.Result.OsID, response.Result.Name, FromApi(response.Result.FleetRegions));
        }

        static FleetInfo.Status FromApi(Fleet.StatusOptions statusOption, string fleetName)
        {
            switch (statusOption)
            {
                case Fleet.StatusOptions.ONLINE:
                    return FleetInfo.Status.Online;
                case Fleet.StatusOptions.DRAINING:
                    return FleetInfo.Status.Draining;
                case Fleet.StatusOptions.OFFLINE:
                    return FleetInfo.Status.Offline;
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(statusOption),
                        statusOption,
                        $"Unrecognized remote fleet status '{statusOption}' from fleet '{fleetName}'");
            }
        }

        static FleetInfo.AllocationStatus FromApi(FleetListItem.ServersAlloc resItemServers)
        {
            return new FleetInfo.AllocationStatus(
                resItemServers.All.Total,
                resItemServers.All.Status.Allocated,
                resItemServers.All.Status.Available,
                resItemServers.All.Status.Online
            );
        }

        static List<FleetInfo.FleetRegionInfo> FromApi(List<FleetRegion1> regions)
        {
            return regions.Select(r => new FleetInfo.FleetRegionInfo(r.RegionID, r.RegionID, r.RegionName)).ToList();
        }

        public async Task Update(
            FleetId id,
            string name,
            IList<BuildConfigurationId> buildConfigurations,
            MultiplayConfig.FleetDefinition definition,
            Guid osId,
            CancellationToken cancellationToken = default)
        {
            var fleet = new FleetUpdateRequest(
                name,
                osId, // Must be set in order to avoid breaking the API
                buildConfigurations.Select(b => b.ToLong()).ToList()
            );
            var request = new UpdateFleetRequest(m_ApiConfig.ProjectId, m_ApiConfig.EnvironmentId, id.ToGuid(), fleet);
            var response = await TryCatchRequestAsync(request, async(req) => {
                return await m_Client.UpdateFleetAsync(req);
            });

            await UpdateRegions(id, response.Result, definition);
        }

        public async Task Clear()
        {
            var listRequest = new ListFleetsRequest(m_ApiConfig.ProjectId, m_ApiConfig.EnvironmentId);
            var fleetsResponse = await TryCatchRequestAsync(listRequest, async(req) => {
                return await m_Client.ListFleetsAsync(req);
            });
            foreach (var fleet in fleetsResponse.Result)
            {
                foreach (var region in fleet.Regions)
                {
                    var updateRequest = new UpdateFleetRegionRequest(
                        m_ApiConfig.ProjectId,
                        m_ApiConfig.EnvironmentId,
                        fleet.Id,
                        region.RegionID,
                        new UpdateRegionRequest(false, 0, 1));
                    await TryCatchRequestAsync(updateRequest, async(req) => {
                        return await m_Client.UpdateFleetRegionAsync(req);
                    });
                }
                var deleteRequest = new DeleteFleetRequest(m_ApiConfig.ProjectId, m_ApiConfig.EnvironmentId, fleet.Id, false);
                await TryCatchRequestAsync(deleteRequest, async(req) => {
                    return await m_Client.DeleteFleetAsync(req);
                });
            }
        }

        async Task UpdateRegions(FleetId id, Fleet fleet, MultiplayConfig.FleetDefinition definition)
        {
            var regions = await GetRegions();

            var existingRegions = fleet.FleetRegions.ToDictionary(k => k.RegionName);
            foreach (var(regionName, region) in definition.Regions)
            {
                if (!existingRegions.ContainsKey(regionName))
                {
                    var regionDefinition = new AddRegionRequest(regions[regionName], region.MinAvailable, region.MaxServers);
                    var regionReq = new AddFleetRegionRequest(
                        m_ApiConfig.ProjectId, m_ApiConfig.EnvironmentId, id.ToGuid(), regionDefinition);
                    await TryCatchRequestAsync(regionReq, async(req) => {
                        return await m_Client.AddFleetRegionAsync(regionReq);
                    });
                }
                else
                {
                    var regionId = existingRegions[regionName].RegionID;
                    var regionDefinition = new UpdateRegionRequest(true, region.MinAvailable, region.MaxServers);
                    var regionReq = new UpdateFleetRegionRequest(
                        m_ApiConfig.ProjectId, m_ApiConfig.EnvironmentId, id.ToGuid(), regionId, regionDefinition);
                    await TryCatchRequestAsync(regionReq, async(req) => {
                        return await m_Client.UpdateFleetRegionAsync(regionReq);
                    });
                }
            }

            foreach (var toRemove in fleet.FleetRegions.Where(r => !definition.Regions.ContainsKey(r.RegionName)))
            {
                var regionReq = new UpdateFleetRegionRequest(m_ApiConfig.ProjectId, m_ApiConfig.EnvironmentId, id.ToGuid(), toRemove.RegionID);
                await TryCatchRequestAsync(regionReq, async(req) => {
                    return await m_Client.UpdateFleetRegionAsync(regionReq);
                });
            }
        }

        async Task<Dictionary<string, Guid>> GetRegions()
        {
            var request = new ListTemplateFleetRegionsRequest(m_ApiConfig.ProjectId, m_ApiConfig.EnvironmentId);
            var response = await TryCatchRequestAsync(request, async(req) => {
                return await m_Client.ListTemplateFleetRegionsAsync(req);
            });
            return response.Result.ToDictionary(r => r.Name, r => r.RegionID);
        }

        async Task<AdminApis.Fleets.Response> TryCatchRequestAsync<TRequest>(TRequest request, Func<TRequest, Task<AdminApis.Fleets.Response>> func)
        {
            AdminApis.Fleets.Response response;
            try
            {
                response = await func(request);
            }
            catch (AdminApis.Fleets.Http.HttpException<InlineResponse400> ex)
            {
                throw new MultiplayAuthoringException((int)ex.Response.StatusCode, ex.ActualError.Detail, ex);
            }
            catch (AdminApis.Fleets.Http.HttpException<InlineResponse401> ex)
            {
                throw new MultiplayAuthoringException((int)ex.Response.StatusCode, ex.ActualError.Detail, ex);
            }
            catch (AdminApis.Fleets.Http.HttpException<InlineResponse403> ex)
            {
                throw new MultiplayAuthoringException((int)ex.Response.StatusCode, ex.ActualError.Detail, ex);
            }
            catch (AdminApis.Fleets.Http.HttpException<InlineResponse404> ex)
            {
                throw new MultiplayAuthoringException((int)ex.Response.StatusCode, ex.ActualError.Detail, ex);
            }
            catch (AdminApis.Fleets.Http.HttpException<InlineResponse429> ex)
            {
                throw new MultiplayAuthoringException((int)ex.Response.StatusCode, ex.ActualError.Detail, ex);
            }
            catch (AdminApis.Fleets.Http.HttpException<InlineResponse500> ex)
            {
                throw new MultiplayAuthoringException((int)ex.Response.StatusCode, "Internal Server Error", ex);
            }
            catch (Exception)
            {
                throw;
            }
            return response;
        }

        async Task<AdminApis.Fleets.Response<TResponse>> TryCatchRequestAsync<TRequest, TResponse>(TRequest request, Func<TRequest, Task<AdminApis.Fleets.Response<TResponse>>> func)
        {
            AdminApis.Fleets.Response<TResponse> response;
            try
            {
                response = await func(request);
            }
            catch (AdminApis.Fleets.Http.HttpException<InlineResponse400> ex)
            {
                throw new MultiplayAuthoringException((int)ex.Response.StatusCode, ex.ActualError.Detail, ex);
            }
            catch (AdminApis.Fleets.Http.HttpException<InlineResponse401> ex)
            {
                throw new MultiplayAuthoringException((int)ex.Response.StatusCode, ex.ActualError.Detail, ex);
            }
            catch (AdminApis.Fleets.Http.HttpException<InlineResponse403> ex)
            {
                throw new MultiplayAuthoringException((int)ex.Response.StatusCode, ex.ActualError.Detail, ex);
            }
            catch (AdminApis.Fleets.Http.HttpException<InlineResponse404> ex)
            {
                throw new MultiplayAuthoringException((int)ex.Response.StatusCode, ex.ActualError.Detail, ex);
            }
            catch (AdminApis.Fleets.Http.HttpException<InlineResponse429> ex)
            {
                throw new MultiplayAuthoringException((int)ex.Response.StatusCode, ex.ActualError.Detail, ex);
            }
            catch (AdminApis.Fleets.Http.HttpException<InlineResponse500> ex)
            {
                throw new MultiplayAuthoringException((int)ex.Response.StatusCode, "Internal Server Error", ex);
            }
            return response;
        }
    }
}
