using System;

namespace VoxelEngine.Core.Assets;

public readonly struct AssetId : IEquatable<AssetId>
{
    public readonly ulong Hash;

    public AssetId(string uri)
    {
        Hash = ComputeHash(uri);
    }

    public AssetId(ulong hash)
    {
        Hash = hash;
    }

    public bool Equals(AssetId other)
    {
        return Hash == other.Hash;
    }

    public override bool Equals(object? obj)
    {
        return obj is AssetId other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Hash.GetHashCode();
    }

    public static bool operator ==(AssetId left, AssetId right) => left.Equals(right);
    public static bool operator !=(AssetId left, AssetId right) => !left.Equals(right);

    // Simple FNV-1a Hash for fast lookup
    private static ulong ComputeHash(string text)
    {
        if (text == null) return 0;
        ulong hash = 14695981039346656037;
        foreach (char c in text)
        {
            hash ^= c;
            hash *= 1099511628211;
        }
        return hash;
    }
}
