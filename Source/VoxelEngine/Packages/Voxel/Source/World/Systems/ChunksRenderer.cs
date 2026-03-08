using System.Runtime.Serialization;
using Arch.Core;
using VoxelEngine.Core;
using VoxelEngine.Graphics;

using VoxelEngine.Graphics.Rendering;
using System.Numerics;
using VoxelEngine.Diagnostics;

namespace VoxelEngine.Packages.Voxel;

public sealed class ChunksRenderer
{

    private readonly ChunksStorage _chunksStorage;
    private readonly RenderManager _renderManager;
    private readonly IGraphicsFactory _factory;

    // private readonly ChunkMaterial _material;
    private readonly PBRMaterial _material;

    public ChunksRenderer(ChunksStorage chunksStorage)
    {
        _chunksStorage = chunksStorage;
        _renderManager = ServiceContainer.Get<RenderManager>()!;
        _factory = ServiceContainer.Get<GraphicsContext>()!.Device.Factory;
        // _material = new ChunkMaterial(new ChunkMaterialProperties(Color.White));
        var assetsManager = ServiceContainer.Get<AssetsManager>()!;
        ShaderData data = assetsManager.GetShaderData(@"C:\Users\Dmitrist10\Desktop\VoxelGames\Source\Project_08_VE\Source\VoxelEngine\Packages\Voxel\Resources\Shaders\ChunkShader.glsl");
        PipelineHandle pipeline = assetsManager.GetOrCreatePipeline(new PipelineDescription(data.Vert, data.Frag));
        _material = new PBRMaterial(new PBRMaterialProperties(Color.White))
        {
            Pipeline = pipeline
        };
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
                    chunk.Mesh = new MeshAsset(_factory.CreateMesh(meshData), meshData.VertexCount, meshData.IndexCount);
            }

            if (chunk.Mesh is { } mesh)
            {
                _renderManager.Submit(new RenderCommand(mesh, _material, Matrix4x4.CreateTranslation(chunk.Position.X, chunk.Position.Y, chunk.Position.Z)));
            }
        }
    }

}
