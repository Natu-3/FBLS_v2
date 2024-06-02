namespace Unity.Services.Multiplay.Authoring.Core.MultiplayApi
{
    struct BuildConfigurationId
    {
        public long Id { private get; init; }

        public long ToLong() => Id;
    }
}
