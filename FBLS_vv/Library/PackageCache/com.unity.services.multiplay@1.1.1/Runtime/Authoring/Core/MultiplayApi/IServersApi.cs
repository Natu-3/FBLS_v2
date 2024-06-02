using System;
using System.Threading;
using System.Threading.Tasks;

namespace Unity.Services.Multiplay.Authoring.Core.MultiplayApi
{
    interface IServersApi
    {
        Task<bool> TriggerServerActionAsync(long serverId, ServerAction action, CancellationToken cancellationToken = default);
    }
}
