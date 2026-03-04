namespace VoxelEngine.Core;

public readonly record struct MeshHandle(ResourceHandle Handle);
public readonly record struct TextureHandle(ResourceHandle Handle);
public readonly record struct BufferHandle(ResourceHandle Handle);
public readonly record struct PipelineHandle(ResourceHandle Handle);
public readonly record struct MaterialHandle(ResourceHandle Handle);
public readonly record struct ShaderHandle(ResourceHandle Handle);