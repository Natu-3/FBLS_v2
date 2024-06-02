namespace Unity.Services.Multiplay.Authoring.Core.Assets
{
    readonly struct BuildName : IResourceName
    {
        public string Name { get; init; }

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object obj)
        {
            return obj is BuildName name && name.Name == Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public static bool operator==(BuildName left, BuildName right)
        {
            return left.Equals(right);
        }

        public static bool operator!=(BuildName left, BuildName right)
        {
            return !(left == right);
        }
    }
}
