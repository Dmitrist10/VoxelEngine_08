using System.Numerics;
using System.Runtime.InteropServices;

namespace VoxelEngine.Core;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct STDVertex : IVertexType
{
    public Vector3 Position; // 12 bytes
    public Vector3 Normal; // 12 bytes
    public Vector2 UV; // 8 bytes

    public STDVertex(Vector3 position, Vector3 normal, Vector2 uv)
    {
        Position = position;
        Normal = normal;
        UV = uv;
    }
    public STDVertex(Vector3 position)
    {
        Position = position;
    }

    public static VertexAttribute[] GetAttributes() => new[]
    {
        new VertexAttribute(0, VertexAttribType.Float3, 0),  // Location 0: Position
        new VertexAttribute(1, VertexAttribType.Float3, 12), // Location 1: Normal
        new VertexAttribute(2, VertexAttribType.Float2, 24)  // Location 2: UV
    };

    public static uint GetStride() => 32;
}
