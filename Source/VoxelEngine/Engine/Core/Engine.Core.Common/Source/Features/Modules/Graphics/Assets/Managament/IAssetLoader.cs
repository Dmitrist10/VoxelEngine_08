using System.IO;

namespace VoxelEngine.Core.Assets;

// Core base interface for DI container
public interface IAssetLoader { }

public interface IAssetLoader<T> : IAssetLoader where T : class, IAssetData
{
    T Load(Stream stream, AssetId id, string absolutePath);
}
