using System;
using System.Collections.Generic;
using VoxelEngine.Core.Assets;
using VoxelEngine.Core.VirtualFileSystem;

namespace VoxelEngine.Core;

public sealed class AssetsManager
{
    private Dictionary<Type, IAssetLoader> _loaders = new Dictionary<Type, IAssetLoader>();

    public VFileManager VFM { get; } = new VFileManager();

    private Dictionary<AssetId, ResourceHandle> _loadedAssets = new Dictionary<AssetId, ResourceHandle>();
    private ResourcePool<IAssetData> _assetPool = new ResourcePool<IAssetData>(1024);

    public AssetsManager()
    {
        VFM.Mount("builtin", new BuiltInFileProvider());
        // Physical file provider should be mounted elsewhere or here with a root path
        VFM.Mount("disk", new PhysicalFileProvider("")); // Disk root

        // Register default loaders
        RegisterLoader(new MeshLoader());
        RegisterLoader(new TextureLoader());
        RegisterLoader(new ShaderLoader());
    }

    public void RegisterLoader<T>(IAssetLoader<T> loader) where T : class, IAssetData
    {
        _loaders[typeof(T)] = loader;
    }

    public AssetHandle<T> Load<T>(string uri) where T : class, IAssetData
    {
        AssetId id = new AssetId(uri);

        if (_loadedAssets.TryGetValue(id, out ResourceHandle handle))
        {
            return new AssetHandle<T>(handle);
        }

        if (!_loaders.TryGetValue(typeof(T), out IAssetLoader? loaderObj) || loaderObj == null)
        {
            throw new Exception($"No loader registered for type {typeof(T).Name}");
        }

        IAssetLoader<T> loader = (IAssetLoader<T>)loaderObj;

        using var stream = VFM.OpenRead(uri);
        string absolutePath = VFM.GetAbsolutePath(uri);

        T assetData = loader.Load(stream, id, absolutePath);

        ResourceHandle newHandle = _assetPool.Add(assetData);
        _loadedAssets[id] = newHandle;

        return new AssetHandle<T>(newHandle);
    }

    public T GetAsset<T>(ResourceHandle handle) where T : class, IAssetData
    {
        return (T)_assetPool.Get(handle);
    }

    public bool IsValid(ResourceHandle handle) => _assetPool.IsValid(handle);

    // Backward compatible shims temporarily for GameSetup and BuildInAssets
    public STDMeshData LoadMesh(string path) => Load<STDMeshData>(path).Get();
    public TextureData LoadTexture(string path, TextureOptions options) => Load<TextureData>(path).Get(); // Temporary options ignorning
    public ShaderData LoadShaderData(string path) => Load<ShaderData>(path).Get();
    public PipelineHandle LoadPipeline(string v) { throw new NotImplementedException(); }
}
