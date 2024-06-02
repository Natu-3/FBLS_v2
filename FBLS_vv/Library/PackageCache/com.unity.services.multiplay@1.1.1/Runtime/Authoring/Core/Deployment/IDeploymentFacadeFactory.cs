using System.Threading.Tasks;

namespace Unity.Services.Multiplay.Authoring.Core.Deployment
{
    interface IDeploymentFacadeFactory
    {
        Task<IDeploymentFacade> BuildAsync();
    }
}
