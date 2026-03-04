using VoxelEngine.Graphics;

namespace VoxelEngine.Core;

public enum VertexAttribType : byte { Float, Float2, Float3, Float4, Int } // only 4byte types

public readonly record struct VertexAttribute(uint Location, VertexAttribType Type, uint Offset);

public interface IVertexType
{
    static abstract VertexAttribute[] GetAttributes();
    static abstract uint GetStride();
}
