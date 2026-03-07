namespace VoxelEngine.Core;

public record struct MeshAsset : IGPUAsset
{
    public MeshHandle Handle;
    public uint VertexCount;
    public uint IndexCount;

    public MeshAsset(MeshHandle handle, uint vertexCount, uint indexCount)
    {
        Handle = handle;
        VertexCount = vertexCount;
        IndexCount = indexCount;
    }
}

public record MeshData<TVertex> : IAssetData where TVertex : unmanaged, IVertexType
{
    public readonly TVertex[] Vertices;
    public readonly uint[] Indices;

    public uint VertexCount => (uint)Vertices.Length;
    public uint IndexCount => (uint)Indices.Length;

    public MeshData(TVertex[] vertices, uint[] indices)
    {
        Vertices = vertices;
        Indices = indices;
    }
}

public record STDMeshData : MeshData<STDVertex>
{
    public STDMeshData(STDVertex[] vertices, uint[] indices) : base(vertices, indices) { }
}
public record VoxelMeshData : MeshData<VoxelVertex>
{
    public VoxelMeshData(VoxelVertex[] vertices, uint[] indices) : base(vertices, indices) { }
}

