using System.Threading.Tasks;
using Unity.Services.Multiplay.Authoring.Core.Builds;
using Unity.Services.Multiplay.Authoring.Core.Logging;
using Unity.Services.Multiplay.Authoring.Core.MultiplayApi;
using Unity.Services.Multiplay.Authoring.Core.Threading;

namespace Unity.Services.Multiplay.Authoring.Core.Deployment
{
    class DeploymentFacadeFactory : IDeploymentFacadeFactory
    {
        readonly IMultiplayBuildAuthoring m_Authoring;
        readonly IBuildFileManagement m_BuildFileManagement;
        readonly ICloudStorageFactory m_CloudStorageFactory;
        readonly IBuildsApiFactory m_BuildsApiFactory;
        readonly IBuildConfigApiFactory m_ConfigApiFactory;
        readonly IFleetApiFactory m_FleetApiFactory;
        readonly IAllocationApiFactory m_AllocationApiFactory;
        readonly IDispatchToMainThread m_Dispatcher;
        readonly ILogger m_Logger;

        public DeploymentFacadeFactory(
            IMultiplayBuildAuthoring authoring,
            IBuildFileManagement buildFileManagement,
            ICloudStorageFactory cloudStorageFactory,
            IBuildsApiFactory buildsApiFactory,
            IBuildConfigApiFactory configApiFactory,
            IFleetApiFactory fleetApiFactory,
            IAllocationApiFactory allocationApiFactory,
            IDispatchToMainThread dispatcher,
            ILogger logger)
        {
            m_Authoring = authoring;
            m_BuildFileManagement = buildFileManagement;
            m_CloudStorageFactory = cloudStorageFactory;
            m_BuildsApiFactory = buildsApiFactory;
            m_ConfigApiFactory = configApiFactory;
            m_FleetApiFactory = fleetApiFactory;
            m_AllocationApiFactory = allocationApiFactory;
            m_Dispatcher = dispatcher;
            m_Logger = logger;
        }

        public async Task<IDeploymentFacade> BuildAsync()
        {
            return new DeploymentFacade(
                m_Authoring,
                m_BuildFileManagement,
                await m_CloudStorageFactory.Build(),
                await m_BuildsApiFactory.Build(),
                await m_ConfigApiFactory.Build(),
                await m_FleetApiFactory.Build(),
                await m_AllocationApiFactory.Build(),
                m_Dispatcher,
                m_Logger);
        }
    }
}
