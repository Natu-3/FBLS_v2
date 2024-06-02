using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Core.Editor.Environments;
using Unity.Services.Multiplay.Authoring.Core.Builds;
using Unity.Services.Multiplay.Authoring.Editor.Analytics;
using Unity.Services.Multiplay.Authoring.Editor.AdminApis.Ccd;
using Unity.Services.Multiplay.Authoring.Editor.AdminApis.Ccd.Apis.Buckets;
using Unity.Services.Multiplay.Authoring.Editor.AdminApis.Ccd.Apis.Entries;
using Unity.Services.Multiplay.Authoring.Editor.AdminApis.Ccd.Http;
using Unity.Services.Multiplay.Authoring.Editor.Shared.Clients;
using UnityEditor;

using ICoreAccessTokens = Unity.Services.Core.Editor.IAccessTokens;

namespace Unity.Services.Multiplay.Authoring.Editor.CloudContent
{
    class CcdCloudStorageFactory : ICloudStorageFactory
    {
        const string k_StgUrl = "https://staging.services.unity.com";

        readonly ICoreAccessTokens m_AccessTokens;
        readonly IEnvironmentsApi m_EnvironmentsApi;

        public CcdCloudStorageFactory(ICoreAccessTokens accessTokens, IEnvironmentsApi environmentsApi)
        {
            m_AccessTokens = accessTokens;
            m_EnvironmentsApi = environmentsApi;
        }

        public async Task<ICloudStorage> Build()
        {
            if (m_EnvironmentsApi.ActiveEnvironmentId == null)
            {
                throw new EnvironmentNotFoundException("Build failed. Please make sure the environment in Project Settings > Environments has been set.");
            }

            var config = new ApiConfig(
                CloudProjectSettings.projectId,
                m_EnvironmentsApi.ActiveEnvironmentId.ToString()
            );

            var gatewayToken = await m_AccessTokens.GetServicesGatewayTokenAsync();

            var client = new HttpClient();
            var configuration = new Configuration(CloudEnvironmentConfigProvider.IsStaging() ? k_StgUrl : null, null, null, new Dictionary<string, string>
            {
                { "Authorization", $"Bearer {gatewayToken}" }
            });

            var bucketsApi = new BucketsApiClient(client, configuration);
            return new CcdCloudStorage(
                config,
                bucketsApi,
                new EntriesApiClient(client, configuration),
                new System.Net.Http.HttpClient(),
                new DummyCcdAnalytics());
        }

        record ApiConfig(string ProjectId, string EnvironmentId) : ICcdApiConfig;
    }
}
