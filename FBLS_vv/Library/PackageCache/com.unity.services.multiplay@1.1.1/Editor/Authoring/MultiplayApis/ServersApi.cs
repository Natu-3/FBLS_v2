using System;
using System.Threading;
using System.Threading.Tasks;
using Unity.Services.Multiplay.Authoring.Core;
using Unity.Services.Multiplay.Authoring.Core.MultiplayApi;
using Unity.Services.Multiplay.Authoring.Editor.AdminApis.Servers.Servers;
using Unity.Services.Multiplay.Authoring.Editor.AdminApis.Servers.Apis.Servers;
using Unity.Services.Multiplay.Authoring.Editor.AdminApis.Servers.Models;

namespace Unity.Services.Multiplay.Authoring.Editor.MultiplayApis
{
    class ServersApi : IServersApi
    {
        readonly IServersApiClient m_Client;
        readonly IMultiplayApiConfig m_ApiConfig;

        public ServersApi(IMultiplayApiConfig apiConfig, IServersApiClient client)
        {
            m_ApiConfig = apiConfig;
            m_Client = client;
        }

        public async Task<bool> TriggerServerActionAsync(long serverId, ServerAction action, CancellationToken cancellationToken = default)
        {
            var actionRequest = new ActionRequest((ActionRequest.ActionOptions)action);
            var serverAction = new TriggerServerActionRequest(
                m_ApiConfig.ProjectId,
                m_ApiConfig.EnvironmentId,
                serverId,
                actionRequest);

            var response = await TryCatchRequestAsync(actionRequest, async(req) => {
                return await m_Client.TriggerServerActionAsync(serverAction);
            });
            return response.Result.Success;
        }

        async Task<AdminApis.Servers.Response<TResponse>> TryCatchRequestAsync<TRequest, TResponse>(TRequest request, Func<TRequest, Task<AdminApis.Servers.Response<TResponse>>> func)
        {
            AdminApis.Servers.Response<TResponse> response;
            try
            {
                response = await func(request);
            }
            catch (AdminApis.Servers.Http.HttpException<InlineResponse400> ex)
            {
                throw new MultiplayAuthoringException((int)ex.Response.StatusCode, ex.ActualError.Detail, ex);
            }
            catch (AdminApis.Servers.Http.HttpException<InlineResponse401> ex)
            {
                throw new MultiplayAuthoringException((int)ex.Response.StatusCode, ex.ActualError.Detail, ex);
            }
            catch (AdminApis.Servers.Http.HttpException<InlineResponse403> ex)
            {
                throw new MultiplayAuthoringException((int)ex.Response.StatusCode, ex.ActualError.Detail, ex);
            }
            catch (AdminApis.Servers.Http.HttpException<InlineResponse404> ex)
            {
                throw new MultiplayAuthoringException((int)ex.Response.StatusCode, ex.ActualError.Detail, ex);
            }
            catch (AdminApis.Servers.Http.HttpException<InlineResponse424> ex)
            {
                throw new MultiplayAuthoringException((int)ex.Response.StatusCode, ex.ActualError.Detail, ex);
            }
            catch (AdminApis.Servers.Http.HttpException<InlineResponse429> ex)
            {
                throw new MultiplayAuthoringException((int)ex.Response.StatusCode, ex.ActualError.Detail, ex);
            }
            catch (AdminApis.Servers.Http.HttpException<InlineResponse500> ex)
            {
                throw new MultiplayAuthoringException((int)ex.Response.StatusCode, "Internal Server Error", ex);
            }
            return response;
        }
    }
}
