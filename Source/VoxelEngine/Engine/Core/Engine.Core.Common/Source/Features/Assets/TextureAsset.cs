namespace VoxelEngine.Core;

public record struct TextureAsset : IAsset
{
    public TextureHandle Handle;
    public uint Width;
    public uint Height;
}
