using System.Numerics;
using System.Runtime.InteropServices;

namespace VoxelEngine.Core;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct VoxelVertex : IVertexType
{
    public Vector3 Position;
    public Vector2 UV;
    public int TextureLayer;
    public int Face;

    public static VertexAttribute[] GetAttributes() => new[]
    {
        new VertexAttribute(0, VertexAttribType.Float3, 0),
        new VertexAttribute(1, VertexAttribType.Float2, 12),
        new VertexAttribute(2, VertexAttribType.Int, 20),
        new VertexAttribute(3, VertexAttribType.Int, 24)
    };
    public static uint GetStride() => 28;

    public VoxelVertex(Vector3 pos, Vector2 uv, int textureLayer, int face) => (Position, UV, TextureLayer, Face) = (pos, uv, textureLayer, face);

}
