using System.Threading.Tasks;

namespace Unity.Services.Multiplay.Authoring.Core.MultiplayApi
{
    interface IBuildConfigApiFactory
    {
        Task<IBuildConfigApi> Build();
    }
}
