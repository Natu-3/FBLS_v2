using System.Threading;
using System.Threading.Tasks;

namespace Unity.Services.Matchmaker.Authoring.Core.Fetch
{
    interface IMatchmakerFetchHandler
    {
        Task<FetchResult> FetchAsync(string rootDir, bool reconcile, bool dryRun, CancellationToken ct = default);
    }
}
