using System.Collections.Generic;

namespace Unity.Services.Multiplay.Authoring.Core.Assets
{
    class MultiplayConfig
    {
        public string Version { get; init; }
        public IDictionary<BuildName, BuildDefinition> Builds { get; init; } = new Dictionary<BuildName, BuildDefinition>();
        public IDictionary<BuildConfigurationName, BuildConfigurationDefinition> BuildConfigurations { get; init; } = new Dictionary<BuildConfigurationName, BuildConfigurationDefinition>();
        public IDictionary<FleetName, FleetDefinition> Fleets { get; init; } = new Dictionary<FleetName, FleetDefinition>();

        public class BuildDefinition
        {
            public string ExecutableName { get; init; } = "Server";
            public string BuildPath { get; set; }
        }

        public class BuildConfigurationDefinition
        {
            public BuildName Build { get; init; }
            public Query? QueryType { get; init; }
            public string BinaryPath { get; init; }
            public string CommandLine { get; init; }
            public IDictionary<string, string> Variables { get; init; } = new Dictionary<string, string>();
            public int Cores { get; init; } = 1;
            public int SpeedMhz { get; init; } = 750;
            public int MemoryMiB { get; init; } = 800;
        }

        public class FleetDefinition
        {
            public IDictionary<string, ScalingDefinition> Regions { get; init; } = new Dictionary<string, ScalingDefinition>();
            public IList<BuildConfigurationName> BuildConfigurations { get; init; } = new List<BuildConfigurationName>();
        }

        public class ScalingDefinition
        {
            public int MinAvailable { get; init; }
            public int MaxServers { get; init; }
        }

        public enum Query
        {
            Sqp,
            A2s,
            None
        }
    }
}
