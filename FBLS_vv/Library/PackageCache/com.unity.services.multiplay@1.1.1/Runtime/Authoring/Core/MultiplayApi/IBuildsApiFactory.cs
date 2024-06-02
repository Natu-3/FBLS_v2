using System.Threading.Tasks;

namespace Unity.Services.Multiplay.Authoring.Core.MultiplayApi
{
    interface IBuildsApiFactory
    {
        Task<IBuildsApi> Build();
    }
}
