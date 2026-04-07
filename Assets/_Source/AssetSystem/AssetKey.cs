// _Source/AssetSystem/AssetKey.cs

using System;

namespace AssetSystem
{
    public readonly struct AssetKey : IEquatable<AssetKey>
    {
        public string Path { get; }
        public Type Type { get; }

        public AssetKey(string path, Type type)
        {
            Path = path;
            Type = type;
        }

        public bool Equals(AssetKey other)
        {
            return Path == other.Path && Type == other.Type;
        }

        public override bool Equals(object obj)
        {
            return obj is AssetKey other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Path, Type);
        }

        public override string ToString()
        {
            return $"[{Type.Name}] {Path}";
        }
    }
}