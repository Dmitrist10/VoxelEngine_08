using System.IO;

namespace VoxelEngine.Core.Assets;

public interface IAssetLoader
{
}

public interface IAssetLoader<T> : IAssetLoader where T : class, IAssetData
{
    T Load(string absolutePath);
}
