namespace Unity.Services.Multiplay.Authoring.Core.MultiplayApi
{
    struct BuildId
    {
        public long Id { private get; init; }

        public long ToLong() => Id;
    }
}
