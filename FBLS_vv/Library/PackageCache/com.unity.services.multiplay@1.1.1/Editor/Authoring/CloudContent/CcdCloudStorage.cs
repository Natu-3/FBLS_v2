using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Unity.Services.Multiplay.Authoring.Core.Builds;
using Unity.Services.Multiplay.Authoring.Editor.Analytics;
using Unity.Services.Multiplay.Authoring.Editor.AdminApis.Ccd;
using Unity.Services.Multiplay.Authoring.Editor.AdminApis.Ccd.Apis.Buckets;
using Unity.Services.Multiplay.Authoring.Editor.AdminApis.Ccd.Apis.Entries;
using Unity.Services.Multiplay.Authoring.Editor.AdminApis.Ccd.Buckets;
using Unity.Services.Multiplay.Authoring.Editor.AdminApis.Ccd.Entries;
using Unity.Services.Multiplay.Authoring.Editor.AdminApis.Ccd.Http;
using Unity.Services.Multiplay.Authoring.Editor.AdminApis.Ccd.Models;
using Unity.Services.Multiplay.Authoring.Core.CloudContent;
using HttpClient = System.Net.Http.HttpClient;

namespace Unity.Services.Multiplay.Authoring.Editor.CloudContent
{
    class CcdCloudStorage : ICloudStorage
    {
        const int k_BatchSize = 10;

        readonly ICcdApiConfig m_ApiConfig;
        readonly IBucketsApiClient m_BucketsApiClient;
        readonly IEntriesApiClient m_EntriesApiClient;
        readonly HttpClient m_UploadClient;
        readonly ICcdAnalytics m_Analytics;

        public CcdCloudStorage(
            ICcdApiConfig apiConfig,
            IBucketsApiClient bucketsApiClient,
            IEntriesApiClient entriesApiClient,
            HttpClient uploadClient,
            ICcdAnalytics analytics)
        {
            m_ApiConfig = apiConfig;
            m_BucketsApiClient = bucketsApiClient;
            m_EntriesApiClient = entriesApiClient;
            m_UploadClient = uploadClient;
            m_Analytics = analytics;
        }

        public async Task<CloudBucketId> FindBucket(string name)
        {
            var request = new ListBucketsByProjectEnvRequest(m_ApiConfig.EnvironmentId, m_ApiConfig.ProjectId, name: name);
            var buckets = await m_BucketsApiClient.ListBucketsByProjectEnvAsync(request);
            return buckets.Result.Select(b => new CloudBucketId { Id = b.Id }).FirstOrDefault();
        }

        public async Task<CloudBucketId> CreateBucket(string name)
        {
            var projectGuid = Guid.Parse(m_ApiConfig.ProjectId);
            var request = new CreateBucketByProjectEnvRequest(m_ApiConfig.EnvironmentId, m_ApiConfig.ProjectId, new InlineObject32(name, projectGuid));
            var res = await m_BucketsApiClient.CreateBucketByProjectEnvAsync(request);
            return new CloudBucketId { Id = res.Result.Id };
        }

        public async Task<int> UploadBuildEntries(CloudBucketId bucket, IList<BuildEntry> localEntries, Action<BuildEntry> onUpdated = null, CancellationToken cancellationToken = default)
        {
            var changes = 0;
            try
            {
                using var upload = m_Analytics.BeginUpload();

                var remoteEntries = await ListAllRemoteEntries(bucket);
                var orphans = remoteEntries.Keys.ToHashSet();

                await localEntries.BatchAsync(k_BatchSize, async local =>
                {
                    var(path, content) = local;
                    var normalizedPath = Normalize(path);
                    var hash = content.CcdHash();
                    if (remoteEntries.ContainsKey(normalizedPath))
                    {
                        orphans.Remove(normalizedPath);
                        if (remoteEntries[normalizedPath].ContentHash.ToLowerInvariant() == hash)
                        {
                            onUpdated?.Invoke(local);
                            return;
                        }
                    }

                    var entry = await CreateOrUpdateEntry(bucket, normalizedPath, hash, (int)content.Length);
                    changes++;
                    await UploadSignedContent(entry, content);
                    onUpdated?.Invoke(local);
                });

                await orphans.BatchAsync(k_BatchSize, async orphan =>
                {
                    var entry = remoteEntries[orphan];
                    changes++;
                    await DeleteEntry(bucket, entry);
                });
            }
            catch (Exception e)
            {
                m_Analytics.UploadFailed(e.GetType().ToString());
                throw;
            }

            return changes;
        }

        public async Task Clear()
        {
            var buckets = await m_BucketsApiClient.ListBucketsByProjectEnvAsync(new ListBucketsByProjectEnvRequest(
                m_ApiConfig.EnvironmentId,
                m_ApiConfig.ProjectId));

            foreach (var bucket in buckets.Result)
            {
                try
                {
                    await m_BucketsApiClient.DeleteBucketEnvAsync(new DeleteBucketEnvRequest(
                        m_ApiConfig.EnvironmentId,
                        bucket.Id.ToString(),
                        m_ApiConfig.ProjectId
                    ));
                }
                catch (HttpException e) when (e.Response.StatusCode == 403)
                {
                    // Ignore Http 403 errors until there is a better way to determine permissions
                    // https://unity.slack.com/archives/CLYNN0XC7/p1660586331118719
                    // Tracked with https://jira.unity3d.com/browse/CCS-2740
                }
            }
        }

        async Task DeleteEntry(CloudBucketId bucket, InlineResponse2003 entry)
        {
            var deleteReq = new DeleteEntryEnvRequest(m_ApiConfig.EnvironmentId, bucket.ToString(), entry.Entryid.ToString(), m_ApiConfig.ProjectId);
            await m_EntriesApiClient.DeleteEntryEnvAsync(deleteReq);
        }

        async Task<InlineResponse2003> CreateOrUpdateEntry(CloudBucketId bucket, string path, string hash, int length)
        {
            var create = new InlineObject23(hash, length, signedUrl: true);
            var request = new CreateOrUpdateEntryByPathEnvRequest(
                m_ApiConfig.EnvironmentId,
                bucket.ToString(),
                path,
                m_ApiConfig.ProjectId,
                create,
                updateIfExists: true);
            var res = await m_EntriesApiClient.CreateOrUpdateEntryByPathEnvAsync(request);
            return res.Result;
        }

        async Task UploadSignedContent(InlineResponse2003 entry, Stream content)
        {
            // Signed uploads need to be done using HTTP Client
            // Unity generated client does not support sending application/offset+octet-stream
            // Timeout and retry is hard to handle here
            var streamContent = new StreamContent(content);
            streamContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/offset+octet-stream");
            var res = await m_UploadClient.PutAsync(entry.SignedUrl, streamContent);
            res.EnsureSuccessStatusCode();
        }

        async Task<IDictionary<string, InlineResponse2003>> ListAllRemoteEntries(CloudBucketId bucket)
        {
            const int entriesPerPage = 100;
            var entries = new Dictionary<string, InlineResponse2003>();

            Response<List<InlineResponse2003>> res;
            var page = 1;
            do
            {
                var request = new GetEntriesEnvRequest(m_ApiConfig.EnvironmentId, bucket.ToString(), m_ApiConfig.ProjectId, page, perPage: entriesPerPage);
                res = await m_EntriesApiClient.GetEntriesEnvAsync(request);

                foreach (var entry in res.Result)
                {
                    entries.Add(entry.Path, entry);
                }
                page++;
            }
            while (res.Result.Count == entriesPerPage);

            return entries;
        }

        static string Normalize(string path)
        {
            return path.TrimStart('/');
        }
    }
}
