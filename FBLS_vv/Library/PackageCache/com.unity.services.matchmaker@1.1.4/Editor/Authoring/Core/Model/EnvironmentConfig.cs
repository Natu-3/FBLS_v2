using System.Runtime.Serialization;

namespace Unity.Services.Matchmaker.Authoring.Core.Model
{
    class EnvironmentConfig : IMatchmakerConfig
    {
        [DataMember(IsRequired = true)]
        public IMatchmakerConfig.ConfigType Type { get; set; } = IMatchmakerConfig.ConfigType.EnvironmentConfig;

        [DataMember(IsRequired = true)] public bool Enabled { get; set; }

        [DataMember(IsRequired = false)] public QueueName DefaultQueueName { get; set; } = new();

    }
}
