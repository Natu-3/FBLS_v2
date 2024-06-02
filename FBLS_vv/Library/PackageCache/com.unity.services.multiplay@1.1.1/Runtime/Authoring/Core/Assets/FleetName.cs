namespace Unity.Services.Multiplay.Authoring.Core.Assets
{
    readonly struct FleetName : IResourceName
    {
        public FleetName(string name) { Name = name; }
        public string Name { get; init; }

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object obj)
        {
            return obj is FleetName name && name.Name == Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
