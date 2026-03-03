using VoxelEngine.Graphics;

namespace VoxelEngine.Core;

public abstract class Material : IAsset
{
    public string Name { get; set; } = "Material.New()";
    // public PipelineAsset pipelineAsset;

    protected readonly IGraphicsFactory _factory;
    protected readonly BufferHandle _BufferHandle;

    protected const uint MATERIAL_BINDING_SLOT = 1;

    public void SetRendering(IGraphicsCommandsList cmdBuffer)
    {
        // cmdBuffer.BindPipeline(pipelineAsset.Handle);
        // cmdBuffer.BindUniformBuffer(_BufferHandle, MATERIAL_BINDING_SLOT);
    }

    public Material()
    {
        _factory = ServiceContainer.Get<GraphicsContext>()!.Device.Factory;
        // _BufferHandle = _factory.CreateBuffer(new BufferDescription());
        // pipelineAsset = 
    }

    public void Dispose()
    {
        // _factory.DestroyBuffer(_BufferHandle);
    }

    // ~Material()
    // {
    //     Dispose();
    // }

}

public class Material<T> : Material, IDisposable where T : unmanaged, IMaterialProperties
{
    public T Properties;

    public Material(T properties)
    {
        Properties = properties;
    }
    public Material() : this(new T()) { }

    public void ApplyChanges()
    {
    }
}


// public sealed class PBRMaterialTesting
// {
//     static void A()
//     {
//         PBRMaterial mat = new();
//         mat.Properties.Color = Color.White;
//         Material<PBRMaterialProperties> mat2 = new();
//         mat2.Properties.Color = Color.White;
//     }
// }
