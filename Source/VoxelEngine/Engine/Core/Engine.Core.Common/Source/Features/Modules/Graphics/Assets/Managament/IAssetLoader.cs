namespace VoxelEngine.Core;

public interface IAssetLoader
{
    IAssetData Load(string path);
}
