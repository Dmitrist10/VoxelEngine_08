namespace VoxelEngine.Core;

public readonly record struct MeshHandle(ResourceHandle Handle) : IAsset;
public readonly record struct TextureHandle(ResourceHandle Handle) : IAsset;
public readonly record struct BufferHandle(ResourceHandle Handle) : IAsset;
public readonly record struct PipelineHandle(ResourceHandle Handle) : IAsset;
public readonly record struct MaterialHandle(ResourceHandle Handle) : IAsset;
public readonly record struct ShaderHandle(ResourceHandle Handle) : IAsset;