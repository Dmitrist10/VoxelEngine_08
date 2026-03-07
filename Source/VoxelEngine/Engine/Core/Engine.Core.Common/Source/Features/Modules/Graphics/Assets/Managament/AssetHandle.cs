using System;
using VoxelEngine.Core;

namespace VoxelEngine.Core.Assets;

public struct AssetHandle<T> : IEquatable<AssetHandle<T>> where T : class, IAssetData
{
    public readonly ResourceHandle Reference;

    public AssetHandle(ResourceHandle reference)
    {
        Reference = reference;
    }

    public T Get()
    {
        var manager = ServiceContainer.Get<AssetsManager>();
        return manager.GetAsset<T>(Reference);
        // Note: AssetsManager will need to implement GetAsset<T>(ResourceHandle) taking advantage of ResourcePool
    }

    public bool IsValid()
    {
        var manager = ServiceContainer.Get<AssetsManager>();
        return manager.IsValid(Reference);
    }

    public bool Equals(AssetHandle<T> other)
    {
        return Reference.Equals(other.Reference);
    }

    public override bool Equals(object obj)
    {
        return obj is AssetHandle<T> other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Reference.GetHashCode();
    }

    public static bool operator ==(AssetHandle<T> left, AssetHandle<T> right) => left.Equals(right);
    public static bool operator !=(AssetHandle<T> left, AssetHandle<T> right) => !left.Equals(right);
}
