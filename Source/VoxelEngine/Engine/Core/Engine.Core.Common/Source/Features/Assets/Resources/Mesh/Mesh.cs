namespace VoxelEngine.Core;

public record struct MeshAsset : IAsset
{
    public static MeshAsset HardcodedTriangleAsset;

    public MeshHandle Handle;
    public uint VertexCount;
    public uint IndexCount;
}

public record MeshData<TVertex> where TVertex : unmanaged, IVertexType
{
    public readonly TVertex[] Vertices;
    public readonly uint[] Indices;

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

