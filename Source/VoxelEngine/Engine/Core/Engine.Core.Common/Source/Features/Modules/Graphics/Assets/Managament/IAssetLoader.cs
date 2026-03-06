namespace VoxelEngine.Core.Assets;

public interface IAssetLoader
{
    IAssetData Load(string path);
}
