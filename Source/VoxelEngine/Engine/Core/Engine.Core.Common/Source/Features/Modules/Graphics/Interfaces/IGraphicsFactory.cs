using VoxelEngine.Core;

namespace VoxelEngine.Graphics;

public interface IGraphicsFactory : IDisposable
{
    IGraphicsCommandsList CreateCommandsList();

    BufferHandle CreateBuffer(in BufferDescription description);
    PipelineHandle CreatePipeline(in PipelineDescription description);
    MeshHandle CreateMesh<T>(MeshData<T> meshData) where T : unmanaged, IVertexType;
    TextureHandle CreateTexture(TextureData textureData);
    TextureHandle CreateTextureArray(Texture2DArrayData textureData);

    void DestroyMesh(MeshHandle handle);
    void DestroyTexture(TextureHandle handle);
    void DestroyBuffer(BufferHandle handle);
}
