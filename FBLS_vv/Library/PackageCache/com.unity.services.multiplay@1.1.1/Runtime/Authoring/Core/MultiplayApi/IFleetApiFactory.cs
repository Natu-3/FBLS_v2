using System.Threading.Tasks;

namespace Unity.Services.Multiplay.Authoring.Core.MultiplayApi
{
    interface IFleetApiFactory
    {
        Task<IFleetApi> Build();
    }
}
