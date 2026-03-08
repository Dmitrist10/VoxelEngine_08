using VoxelEngine.Core;
using Silk.NET.OpenGL;

namespace VoxelEngine.Graphics.OpenGL;

internal sealed class GL_AssetsManager
{
    private readonly ResourcePool<GL_Mesh> _meshPool = new();
    private readonly ResourcePool<GL_Pipeline> _pipelinePool = new();
    private readonly ResourcePool<GL_Buffer> _bufferPool = new();
    private readonly ResourcePool<GL_TextureResource> _texturePool = new();

    [MethodImpl(AggressiveInlining)]
    internal GL_Mesh Get(MeshHandle handle)
    {
        return _meshPool.Get(handle.Handle);
    }
    [MethodImpl(AggressiveInlining)]
    internal GL_Pipeline Get(PipelineHandle handle)
    {
        return _pipelinePool.Get(handle.Handle);
    }
    [MethodImpl(AggressiveInlining)]
    internal GL_Buffer Get(BufferHandle handle)
    {
        return _bufferPool.Get(handle.Handle);
    }
    [MethodImpl(AggressiveInlining)]
    internal GL_TextureResource Get(TextureHandle handle)
    {
        return _texturePool.Get(handle.Handle);
    }

    [MethodImpl(AggressiveInlining)]
    internal MeshHandle Add(GL_Mesh mesh)
    {
        return new(_meshPool.Add(mesh));
    }
    [MethodImpl(AggressiveInlining)]
    internal PipelineHandle Add(GL_Pipeline pipeline)
    {
        return new(_pipelinePool.Add(pipeline));
    }
    [MethodImpl(AggressiveInlining)]
    internal BufferHandle Add(GL_Buffer buffer)
    {
        return new(_bufferPool.Add(buffer));
    }
    [MethodImpl(AggressiveInlining)]
    internal TextureHandle Add(GL_TextureResource texture)
    {
        return new(_texturePool.Add(texture));
    }

    [MethodImpl(AggressiveInlining)]
    internal void Remove(MeshHandle handle)
    {
        _meshPool.Remove(handle.Handle);
    }
    [MethodImpl(AggressiveInlining)]
    internal void Remove(PipelineHandle handle)
    {
        _pipelinePool.Remove(handle.Handle);
    }
    [MethodImpl(AggressiveInlining)]
    internal void Remove(BufferHandle handle)
    {
        _bufferPool.Remove(handle.Handle);
    }
    [MethodImpl(AggressiveInlining)]
    internal void Remove(TextureHandle handle)
    {
        _texturePool.Remove(handle.Handle);
    }
}

internal record GL_Buffer(uint ID, uint Size);

internal record GL_Pipeline(uint ID);
internal record GL_Mesh(uint VAO, uint VBO, uint EBO);

internal enum GL_TextureType : byte { Texture2D, Texture2DArray }
internal record GL_TextureResource(uint ID, uint Width, uint Height, GL_TextureType Type, uint Layers = 0);