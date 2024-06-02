using System.Threading.Tasks;

namespace Unity.Services.Multiplay.Authoring.Core.MultiplayApi
{
    interface IServersApiFactory
    {
        Task<IServersApi> Build();
    }
}
