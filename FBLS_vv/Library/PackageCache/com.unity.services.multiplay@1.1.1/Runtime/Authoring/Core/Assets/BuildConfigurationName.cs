namespace Unity.Services.Multiplay.Authoring.Core.Assets
{
    readonly struct BuildConfigurationName : IResourceName
    {
        public string Name { get; init; }

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object obj)
        {
            return obj is BuildConfigurationName name && name.Name == Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
