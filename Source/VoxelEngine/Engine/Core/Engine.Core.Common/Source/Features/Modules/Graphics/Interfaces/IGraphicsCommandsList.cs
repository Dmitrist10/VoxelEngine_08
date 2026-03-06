using VoxelEngine.Core;

namespace VoxelEngine.Graphics;

public interface IGraphicsCommandsList : IDisposable
{
    void Begin();
    void End();

    void BindPipeline(PipelineHandle pipeline);
    void BindMesh(MeshHandle mesh);
    void BindUniformBuffer(BufferHandle buffer, uint bindingSlot = 0);

    void UpdateBuffer<T>(BufferHandle buffer, uint offset, ref T data) where T : unmanaged;
    // void BindDescriptorSet(DescriptorSetHandle descriptorSet);
    // void BindVertexBuffer(VertexBufferHandle vertexBuffer);
    // void BindIndexBuffer(IndexBufferHandle indexBuffer);
    // void BindShader(ShaderHandle shader);

    // void Draw(uint vertexCount);
    void DrawIndexed(uint indexCount, PrimitiveTopology topology = PrimitiveTopology.Triangles);

    void ClearColor(Color color);
}