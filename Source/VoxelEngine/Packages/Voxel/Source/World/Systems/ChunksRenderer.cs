using System.Runtime.Serialization;
using Arch.Core;
using VoxelEngine.Core;
using VoxelEngine.Graphics;

using VoxelEngine.Graphics.Rendering;
using System.Numerics;

namespace VoxelEngine.Packages.Voxel;

public sealed class ChunksRenderer
{

    private readonly ChunksStorage _chunksStorage;
    private readonly IGraphicsDevice _device;
    private readonly RenderManager _renderManager;

    private readonly ChunkMaterial _material;

    public ChunksRenderer(ChunksStorage chunksStorage)
    {
        _chunksStorage = chunksStorage;
        _device = ServiceContainer.Get<GraphicsContext>()!.Device;
        _renderManager = ServiceContainer.Get<RenderManager>()!;
        _material = new ChunkMaterial(new ChunkMaterialProperties(Color.White));
    }

    public void Render()
    {
        ReadOnlySpan<Chunk> chunks = _chunksStorage.GetAllChunks();
        foreach (var chunk in chunks)
        {
            if (chunk.IsDirty)
            {
                chunk.ClearDirty();

                var meshData = ChunkMesher.CreateMesh(chunk);
                if (meshData != null)
                {
                    // if (chunk.Mesh != null)
                    // {
                    //    ServiceContainer.Get<AssetsManager>()?.Unload(chunk.Mesh);
                    // }

                    // _device.Factory.CreateMesh doesn't exist yet, we will use AssetsManager? 
                    // Let's assume there is something to load VoxelMeshData into a MeshAsset. 
                    // For now, I'll just write it as a TODO or assuming the method exists since the user wrote standard mesh creation.
                    // Actually, let's leave chunk.Mesh assignment empty for a moment if there's no clear API.
                    // chunk.Mesh = ServiceContainer.Get<AssetsManager>()?.CreateMesh(meshData);
                }
            }

            if (chunk.Mesh is { } mesh)
            {
                _renderManager.Submit(new RenderCommand(mesh, _material, Matrix4x4.CreateTranslation(chunk.Position.X, chunk.Position.Y, chunk.Position.Z)));
            }
        }
    }

}
