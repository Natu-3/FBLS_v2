using System;
using Unity.Services.Core.Editor;
using Unity.Services.Core.Editor.Environments;
using Unity.Services.DeploymentApi.Editor;
using Unity.Services.Multiplay.Authoring.Core;
using Unity.Services.Multiplay.Authoring.Core.Assets;
using Unity.Services.Multiplay.Authoring.Core.Builds;
using Unity.Services.Multiplay.Authoring.Core.Deployment;
using Unity.Services.Multiplay.Authoring.Core.Logging;
using Unity.Services.Multiplay.Authoring.Core.MultiplayApi;
using Unity.Services.Multiplay.Authoring.Core.Threading;
using Unity.Services.Multiplay.Authoring.Editor.AdminApis.Ccd.Scheduler;
using Unity.Services.Multiplay.Authoring.Editor.Analytics;
using Unity.Services.Multiplay.Authoring.Editor.Assets;
using Unity.Services.Multiplay.Authoring.Editor.Builds;
using Unity.Services.Multiplay.Authoring.Editor.CloudContent;
using Unity.Services.Multiplay.Authoring.Editor.Deployment;
using Unity.Services.Multiplay.Authoring.Editor.Logging;
using Unity.Services.Multiplay.Authoring.Editor.MultiplayApis;
using Unity.Services.Multiplay.Authoring.Editor.Shared.Analytics;
using Unity.Services.Multiplay.Authoring.Editor.Shared.Assets;
using Unity.Services.Multiplay.Authoring.Editor.Shared.Chrono;
using Unity.Services.Multiplay.Authoring.Editor.Shared.DependencyInversion;
using UnityEditor;
using static Unity.Services.Multiplay.Authoring.Editor.Shared.DependencyInversion.Factories;
using IAccessTokens = Unity.Services.Core.Editor.IAccessTokens;

namespace Unity.Services.Multiplay.Authoring.Editor
{
    static class MultiplayAuthoringServices
    {
        public static IScopedServiceProvider Provider { get; }

        static MultiplayAuthoringServices()
        {
            var collection = new ServiceCollection(true);
            Register(collection);
            Provider = collection.Build();
        }

        [InitializeOnLoadMethod]
        static void RegisterDeploymentProvider()
        {
            Deployments.Instance.DeploymentProviders.Add(Provider.GetService<DeploymentProvider>());
        }

        internal static void Register(ServiceCollection collection)
        {
            collection.Register(Default<IFileReader, FileReader>);
            collection.Register(Default<IBuildFileManagement, BuildFileManagement>);
            collection.Register(Default<IBinaryBuilder, BinaryBuilder>);
            collection.Register(Default<IMultiplayBuildAuthoring, MultiplayBuildAuthoring>);
            collection.Register(Default<IMultiplayConfigValidator, MultiplayConfigValidator>);
            collection.Register(Default<ICloudStorageFactory, CcdCloudStorageFactory>);
            collection.Register(Default<IFleetApiFactory, MultiplayApiFactory>);
            collection.Register(Default<IBuildsApiFactory, MultiplayApiFactory>);
            collection.Register(Default<IBuildConfigApiFactory, MultiplayApiFactory>);
            collection.Register(Default<IAllocationApiFactory, MultiplayApiFactory>);
            collection.Register(Default<IServersApiFactory, MultiplayApiFactory>);

            collection.Register(Default<IAccessTokens, AccessTokens>);
            collection.Register(Default<ICurrentTime, CurrentTime>);

            collection.Register(Default<IAssetAnalytics, AssetAnalytics>);
            collection.Register(Default<IBuildsAnalytics, BuildsAnalytics>);
            collection.Register(Default<IDeployAnalytics, DeployAnalytics>);
            collection.Register(Default<ICommonAnalytics, CommonAnalytics>);
            collection.Register(Default<IAnalyticsUtils, AnalyticsUtils>);
#if UNITY_2023_2_OR_NEWER
            collection.Register(Default<ICommonAnalyticProvider, CommonAnalyticProvider>);
#endif

            collection.RegisterSingleton(Default<ObservableAssets<MultiplayConfigAsset>, MultiplayConfigObservableAssets>);


            collection.Register(Default<IDispatchToMainThread, DispatcherToMainThread>);
            collection.Register(Default<ITaskDelay, DefaultTaskDelay>);
            collection.Register(Default<MultiplayDeployer>);
            collection.Register(Default<IDeploymentFacadeFactory, DeploymentFacadeFactory>);

            collection.RegisterSingleton<IItemStore>(_ => AssetPersistenceStore.Instance);
            collection.RegisterSingleton(_ => EnvironmentsApi.Instance);

            collection.RegisterSingleton<DeploymentProvider>(sp =>
            {
                var provider = new MultiplayDeploymentProvider(
                    new MultiplayConfigObservableAssets(new AssetAnalytics(sp.GetService<IAnalyticsUtils>())),
                    new DeployCommand(sp.GetService<IDeployAnalytics>()),
                    sp.GetService<IItemStore>()
                );
                provider.Commands.Add(new BuildCommand());
                provider.Commands.Add(new UploadCommand());
                return provider;
            });

            collection.Register(Default<ILogger, Logger>);
            collection.RegisterStartupSingleton(Default<FleetStatusTracker>);
        }

        internal static T GetService<T>(this IServiceProvider provider)
        {
            return (T)provider.GetService(typeof(T));
        }
    }
}
