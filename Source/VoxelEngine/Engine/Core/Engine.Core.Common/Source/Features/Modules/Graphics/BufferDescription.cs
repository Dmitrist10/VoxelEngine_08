namespace VoxelEngine.Graphics;

public enum BufferUsage : byte
{
    VertexBuffer,
    IndexBuffer,
    UniformBuffer
}

public struct BufferDescription
{
    public uint Size;
    public BufferUsage Usage;
    public nint Data;
}
