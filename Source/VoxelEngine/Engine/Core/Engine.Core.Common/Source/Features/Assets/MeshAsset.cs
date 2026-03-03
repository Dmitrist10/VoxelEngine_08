namespace VoxelEngine.Core;

public record struct MeshAsset : IAsset
{
    public MeshHandle Handle;
    public uint VertexCount;
    public uint IndexCount;
}
