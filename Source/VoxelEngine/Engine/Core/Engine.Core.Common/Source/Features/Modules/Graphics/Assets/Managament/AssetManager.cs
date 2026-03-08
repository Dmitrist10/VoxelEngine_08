using System;
using System.Collections.Generic;
using System.Numerics;
using VoxelEngine.Audio;
using VoxelEngine.Core.Assets;
using VoxelEngine.Graphics;

namespace VoxelEngine.Core;

public sealed class AssetsManager
{

    private struct AssetsDatabaseItem
    {
        // public AssetId Id;
        public string VirtualPath;
        public ushort ReferenceCount;
        public IAsset? Asset;

        public AssetsDatabaseItem(string virtualPath, IAsset asset, ushort referenceCount = 1) => (VirtualPath, Asset, ReferenceCount) = (virtualPath, asset, referenceCount);
        public AssetsDatabaseItem(string virtualPath, ushort referenceCount = 1) => (VirtualPath, ReferenceCount) = (virtualPath, referenceCount);

        public AssetsDatabaseItem Clone() => new AssetsDatabaseItem() { VirtualPath = VirtualPath, ReferenceCount = 0, Asset = Asset };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddReference() => ReferenceCount++;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveReference() => ReferenceCount--;


        public bool IsValid => ReferenceCount > 0;
    }

    private IGraphicsFactory _factory;
    private Dictionary<string, AssetsDatabaseItem> _assetsCache = new Dictionary<string, AssetsDatabaseItem>();  // Virtual path -> Asset

    private readonly MeshLoader _meshLoader;
    private readonly ShaderLoader _shaderLoader;
    private readonly TextureLoader _textureLoader;
    private readonly AudioLoader _audioLoader;

    public AssetsManager()
    {
        _factory = ServiceContainer.Get<GraphicsContext>()!.Device.Factory;
        _meshLoader = new MeshLoader();
        _shaderLoader = new ShaderLoader();
        _textureLoader = new TextureLoader();
        _audioLoader = new AudioLoader();
    }

    public MeshAsset GetMesh(string virtualPath)
    {
        if (_assetsCache.TryGetValue(virtualPath, out AssetsDatabaseItem item)) // search if we have already loaded asset
            return (MeshAsset)item.Asset!;

        STDMeshData data = GetMeshData(virtualPath);
        MeshHandle handle = _factory.CreateMesh(data);
        MeshAsset asset = new MeshAsset(handle, data.VertexCount, data.IndexCount);

        _assetsCache.Add(virtualPath, new AssetsDatabaseItem(virtualPath, asset));
        return asset;
    }
    public STDMeshData GetMeshData(string virtualPath)
    {
        // if (_assetsCache.TryGetValue(virtualPath, out AssetsDatabaseItem item)) // search if we have already loaded asset
        //     return (STDMeshData)item.Asset!;

        STDMeshData data = _meshLoader.Load(virtualPath);
        // _assetsCache.Add(virtualPath, new AssetsDatabaseItem(virtualPath, data));
        return data;
    }

    public ReadOnlySpan<Vector3> GetMeshVerteciesPositions(string virtualPath)
    {
        throw new NotImplementedException();
    }

    public ShaderData GetShaderData(string virtualPath)
    {
        if (_assetsCache.TryGetValue(virtualPath, out AssetsDatabaseItem item)) // search if we have already loaded asset
            return (ShaderData)item.Asset!;

        ShaderData data = _shaderLoader.Load(virtualPath);
        _assetsCache.Add(virtualPath, new AssetsDatabaseItem(virtualPath, data));
        return data;
    }

    // public Material GetMaterial(string virtualPath)
    // {
    //     if (_assetsDatabase.TryGetValue(virtualPath, out AssetsDatabaseItem item)) // search if we have already loaded asset
    //         return (Material)item.Asset!;


    // }
    public PBRMaterial GetPBRMaterial(string virtualPath)
    {
        throw new NotImplementedException();
    }
    public TextureMaterial GetTextureMaterial(string virtualPath)
    {
        throw new NotImplementedException();
    }

    public AudioAsset GetAudio(string virtualPath)
    {
        if (_assetsCache.TryGetValue(virtualPath, out AssetsDatabaseItem item)) // search if we have already loaded asset
            return (AudioAsset)item.Asset!;

        AudioData data = _audioLoader.Load(virtualPath);
        AudioHandle handle = AudioManager.LoadAudioBuffer(data);
        AudioAsset asset = new(handle, data);

        _assetsCache.Add(virtualPath, new AssetsDatabaseItem(virtualPath, asset));
        return asset;
    }
    public AudioData GetAudioData(string virtualPath)
    {
        return _audioLoader.Load(virtualPath);
    }


    public TextureAsset GetTexture(string virtualPath)
    {
        if (_assetsCache.TryGetValue(virtualPath, out AssetsDatabaseItem item)) // search if we have already loaded asset
            return (TextureAsset)item.Asset!;

        TextureData data = _textureLoader.Load(virtualPath);
        TextureHandle handle = _factory.CreateTexture(data);
        TextureAsset asset = new TextureAsset(handle, data.Width, data.Height);

        _assetsCache.Add(virtualPath, new AssetsDatabaseItem(virtualPath, asset));
        return asset;
    }

    public PipelineHandle GetOrCreatePipeline(PipelineDescription desc)
    {
        string str = desc.ToString();
        if (_assetsCache.TryGetValue(str, out AssetsDatabaseItem item)) // search if we have already cached asset
            return (PipelineHandle)item.Asset!;

        PipelineHandle pipeline = _factory.CreatePipeline(desc);
        _assetsCache.Add(str, new AssetsDatabaseItem(str, pipeline));
        return pipeline;
    }

}
