using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.Services.Multiplay.Authoring.Core;
using Unity.Services.Multiplay.Authoring.Core.Assets;
using Unity.Services.Multiplay.Authoring.Core.Builds;
using Unity.Services.Multiplay.Authoring.Core.MultiplayApi;
using Unity.Services.Multiplay.Authoring.Editor.AdminApis.Builds.Apis.Builds;
using Unity.Services.Multiplay.Authoring.Editor.AdminApis.Builds.Builds;
using Unity.Services.Multiplay.Authoring.Editor.AdminApis.Builds.Models;
using CreateBuildRequest = Unity.Services.Multiplay.Authoring.Editor.AdminApis.Builds.Builds.CreateBuildRequest;
using CreateNewBuildVersionRequest = Unity.Services.Multiplay.Authoring.Editor.AdminApis.Builds.Builds.CreateNewBuildVersionRequest;

namespace Unity.Services.Multiplay.Authoring.Editor.MultiplayApis
{
    class BuildsApi : IBuildsApi
    {
        readonly IMultiplayApiConfig m_ApiConfig;
        readonly IBuildsApiClient m_Client;

        public BuildsApi(IMultiplayApiConfig apiConfig, IBuildsApiClient client)
        {
            m_ApiConfig = apiConfig;
            m_Client = client;
        }

        public async Task<(BuildId, CloudBucketId)?> FindByName(string name, CancellationToken cancellationToken = default)
        {
            var request = new ListBuildsRequest(m_ApiConfig.ProjectId, m_ApiConfig.EnvironmentId, partialFilter: name);
            var response = await TryCatchRequestAsync(request, async(req) => {
                return await m_Client.ListBuildsAsync(req);
            });

            var result = response.Result.FirstOrDefault(
                b => b.BuildName == name);

            if (result == null)
                return null;

            return (new BuildId {Id = result.BuildID },
                new CloudBucketId {Id = result.Ccd?.BucketID ?? Guid.Empty });
        }

        public async Task<(BuildId, CloudBucketId)> Create(string name, MultiplayConfig.BuildDefinition definition, CancellationToken cancellationToken = default)
        {
            var request = new CreateBuildRequest(
                m_ApiConfig.ProjectId,
                m_ApiConfig.EnvironmentId,
                new Unity.Services.Multiplay.Authoring.Editor.AdminApis.Builds.Models.CreateBuildRequest(
                    name,
                    buildType: AdminApis.Builds.Models.CreateBuildRequest.BuildTypeOptions.FILEUPLOAD)
            );
            var response = await TryCatchRequestAsync(request, async(req) => {
                return await m_Client.CreateBuildAsync(req);
            });
            return (
                new BuildId { Id = response.Result.BuildID },
                new CloudBucketId { Id = response.Result.Ccd?.BucketID ?? Guid.Empty }
            );
        }

        public async Task CreateVersion(BuildId id, CloudBucketId bucket, CancellationToken cancellationToken = default)
        {
            var request = new CreateNewBuildVersionRequest(
                m_ApiConfig.ProjectId,
                m_ApiConfig.EnvironmentId,
                id.ToLong(),
                new Unity.Services.Multiplay.Authoring.Editor.AdminApis.Builds.Models.CreateNewBuildVersionRequest(
                    new CCDDetails2(bucket.ToGuid())
                )
            );
            await TryCatchRequestAsync(request, async(req) => {
                return await m_Client.CreateNewBuildVersionAsync(req);
            });
        }

        public async Task<bool> IsSynced(BuildId id, CancellationToken cancellationToken = default)
        {
            var request = new GetBuildRequest(m_ApiConfig.ProjectId, m_ApiConfig.EnvironmentId, id.ToLong());
            var response = await TryCatchRequestAsync(request, async(req) => {
                return await m_Client.GetBuildAsync(req);
            });

            if (response.Result.SyncStatus == CreateBuild200Response.SyncStatusOptions.FAILED)
            {
                throw new SyncFailedException();
            }
            return response.Result.SyncStatus == CreateBuild200Response.SyncStatusOptions.SYNCED;
        }

        public async Task Clear()
        {
            var request = new ListBuildsRequest(m_ApiConfig.ProjectId, m_ApiConfig.EnvironmentId);
            var response = await TryCatchRequestAsync(request, async(req) => {
                return await m_Client.ListBuildsAsync(req);
            });
            foreach (var build in response.Result)
            {
                try
                {
                    var delete = new DeleteBuildRequest(m_ApiConfig.ProjectId, m_ApiConfig.EnvironmentId, build.BuildID, dryRun: false);
                    await m_Client.DeleteBuildAsync(delete);
                }
                catch (Exception)
                {
                    // Ignored because some builds can not be removed
                }
            }
        }

        async Task<AdminApis.Builds.Response<TResponse>> TryCatchRequestAsync<TRequest, TResponse>(TRequest request, Func<TRequest, Task<AdminApis.Builds.Response<TResponse>>> func)
        {
            AdminApis.Builds.Response<TResponse> response;
            try
            {
                response = await func(request);
            }
            // spec is missing 400 response
            // catch (AdminApis.Builds.Http.HttpException<ListBuilds400Response> ex)
            // {
            //     throw new MultiplayAuthoringException((int)ex.Response.StatusCode, ex.ActualError.Detail, ex);
            // }
            catch (AdminApis.Builds.Http.HttpException<ListBuilds401Response> ex)
            {
                throw new MultiplayAuthoringException((int)ex.Response.StatusCode, ex.ActualError.Detail, ex);
            }
            catch (AdminApis.Builds.Http.HttpException<ListBuilds403Response> ex)
            {
                throw new MultiplayAuthoringException((int)ex.Response.StatusCode, ex.ActualError.Detail, ex);
            }
            catch (AdminApis.Builds.Http.HttpException<ListBuilds404Response> ex)
            {
                throw new MultiplayAuthoringException((int)ex.Response.StatusCode, ex.ActualError.Detail, ex);
            }
            catch (AdminApis.Builds.Http.HttpException<ListBuilds429Response> ex)
            {
                throw new MultiplayAuthoringException((int)ex.Response.StatusCode, ex.ActualError.Detail, ex);
            }
            catch (AdminApis.Builds.Http.HttpException<ListBuilds500Response> ex)
            {
                throw new MultiplayAuthoringException((int)ex.Response.StatusCode, "Internal Server Error", ex);
            }
            return response;
        }
    }
}
