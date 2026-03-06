using Veldrid;
using VoxelEngine.Core;

namespace VoxelEngine.Graphics.Veldrid;

internal sealed class VEL_AssetsManager
{
    private readonly ResourcePool<VEL_Mesh> _meshPool = new();
    private readonly ResourcePool<VEL_Pipeline> _pipelinePool = new();
    private readonly ResourcePool<VEL_Buffer> _bufferPool = new();

    [MethodImpl(AggressiveInlining)]
    internal VEL_Mesh Get(MeshHandle handle) => _meshPool.Get(handle.Handle);

    [MethodImpl(AggressiveInlining)]
    internal VEL_Pipeline Get(PipelineHandle handle) => _pipelinePool.Get(handle.Handle);

    [MethodImpl(AggressiveInlining)]
    internal VEL_Buffer Get(BufferHandle handle) => _bufferPool.Get(handle.Handle);


    [MethodImpl(AggressiveInlining)]
    internal MeshHandle Add(VEL_Mesh mesh) => new(_meshPool.Add(mesh));

    [MethodImpl(AggressiveInlining)]
    internal PipelineHandle Add(VEL_Pipeline pipeline) => new(_pipelinePool.Add(pipeline));

    [MethodImpl(AggressiveInlining)]
    internal BufferHandle Add(VEL_Buffer buffer) => new(_bufferPool.Add(buffer));
}

// ---------------------------------------------------------------------------
// Internal resource wrappers around Veldrid objects.
// These hold the actual Veldrid handles; the rest of the engine never sees them.
// ---------------------------------------------------------------------------

/// <summary>A Veldrid-backed GPU buffer (vertex, index, or uniform).</summary>
internal sealed class VEL_Buffer(DeviceBuffer buffer, uint size) : IDisposable
{
    public readonly DeviceBuffer Buffer = buffer;
    public readonly uint Size = size;
    public void Dispose() => Buffer.Dispose();
}

/// <summary>A compiled Veldrid pipeline (shader program + render state + resource layout).</summary>
internal sealed class VEL_Pipeline(Pipeline pipeline, ResourceLayout resourceLayout, Shader[] shaders) : IDisposable
{
    public readonly Pipeline Pipeline = pipeline;
    public readonly ResourceLayout ResourceLayout = resourceLayout;
    public readonly Shader[] Shaders = shaders;
    public void Dispose()
    {
        Pipeline.Dispose();
        ResourceLayout.Dispose();
        foreach (Shader s in Shaders) s.Dispose();
    }
}

/// <summary>A mesh composed of a vertex buffer and an index buffer.</summary>
internal sealed class VEL_Mesh(DeviceBuffer vertexBuffer, DeviceBuffer indexBuffer, uint indexCount) : IDisposable
{
    public readonly DeviceBuffer VertexBuffer = vertexBuffer;
    public readonly DeviceBuffer IndexBuffer = indexBuffer;
    public readonly uint IndexCount = indexCount;
    public void Dispose()
    {
        VertexBuffer.Dispose();
        IndexBuffer.Dispose();
    }
}