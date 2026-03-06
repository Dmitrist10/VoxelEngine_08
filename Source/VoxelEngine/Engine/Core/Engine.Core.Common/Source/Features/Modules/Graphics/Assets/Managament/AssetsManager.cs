namespace VoxelEngine.Core;

public sealed class AssetsManager
{
    private Dictionary<Type, IAssetLoader> _loaders = new Dictionary<Type, IAssetLoader>();

    public void RegisterLoader<T>(IAssetLoader loader) where T : IAssetData
    {
        _loaders.Add(typeof(T), loader);
    }

    public T LoadAsset<T>(string v)
    {
        throw new NotImplementedException();
    }

    public MeshAsset LoadMesh(string path)
    {
        return new MeshAsset();
    }

    public ShaderData LoadShaderData(string path)
    {
        return (ShaderData)_loaders[typeof(ShaderData)].Load(path);
    }

    public PipelineHandle LoadPipeline(string v)
    {
        throw new NotImplementedException();
    }
    // public STDMeshData LoadMeshData(string path)
    // {
    //     return new STDMeshData();
    // }

    // public TextureAsset LoadTexture(string path)
    // {
    //     return new TextureAsset();
    // }

    // public ShaderAsset LoadShader(string path)
    // {
    //     return new ShaderAsset();
    // }

    // public PipelineAsset LoadPipeline(string path)
    // {
    //     return new PipelineAsset();
    // }

    // public Material LoadMaterial(string path)
    // {
    //     return new Material();
    // }

}
