using System.Diagnostics;
using System.Numerics;
using VoxelEngine.Core;
using VoxelEngine.Diagnostics;
using VoxelEngine.Graphics;
using VoxelEngine.Graphics.Rendering;

namespace VoxelEngine.Packages.Voxel;

public sealed class ChunksRenderer : IRenderable
{
    private RenderManager renderManager;
    private IGraphicsFactory factory;

    private VoxelWorld _voxelWorld;
    private World _world;

    private ChunkMaterial _material;

    public ChunksRenderer(VoxelWorld ve, World world)
    {
        _voxelWorld = ve;
        _world = world;
        renderManager = ServiceContainer.Get<RenderManager>()!;
        factory = ServiceContainer.Get<GraphicsContext>()!.Device.Factory;

        var assetsManager = ServiceContainer.Get<AssetsManager>()!;
        ShaderData data = assetsManager.GetShaderData(@"C:\Users\Dmitrist10\Desktop\VoxelGames\Source\Project_08_VE\Source\VoxelEngine\Packages\Voxel\Resources\Shaders\ChunkShader.glsl");
        PipelineHandle pipeline = assetsManager.GetOrCreatePipeline(new PipelineDescription(data.Vert, data.Frag));

        string basePath = @"C:\Users\Dmitrist10\Desktop\VoxelGames\Source\Project_08_VE\Source\Resources\Games\Game_01\Assets\Blocks";
        string[] chunkTextures = new string[] {
            Path.Combine(basePath, "test.png"),              // Layer 0: Test Block
            Path.Combine(basePath, "Floor_1_overgrown.png"), // Layer 1: Grass
            Path.Combine(basePath, "Floor_2.png"),           // Layer 2: Dirt
            Path.Combine(basePath, "Stone_01.png"),          // Layer 3: Stone
            Path.Combine(basePath, "Bricks_01.png"),         // Layer 4: Bricks
            Path.Combine(basePath, "Grass_01.png"),          // Layer 5: Grass
            Path.Combine(basePath, "LuckyBlock_01.png"),     // Layer 6: LuckyBlock
            Path.Combine(basePath, "Dirt_01.png")            // Layer 7: Dirt
        };

        Texture2DArrayData textureData = TextureArrayBuilder.Build(chunkTextures, TextureOptions.VoxelAtlas);
        TextureHandle handle = factory.CreateTextureArray(textureData);

        _material = new ChunkMaterial(new ChunkMaterialProperties(Color.White))
        {
            Pipeline = pipeline,
            AlbedoTexture = handle
        };
        // _material = new PBRMaterial(new PBRMaterialProperties(Color.Red))
        // {
        //     Pipeline = pipeline,
        // };
    }

    public void OnRender()
    {
        // Stopwatch watch = new Stopwatch();
        // watch.Start();
        ReadOnlySpan<Chunk> chunks = _voxelWorld.ChunksStorage.GetAllChunks();

        foreach (Chunk c in chunks)
        {
            if (c.IsDirty)
            {
                VoxelMeshData? data = ChunkMesher.CreateMesh(c);
                c.ClearDirty(); // Ensure we don't try to mesh it every frame
                if (data == null)
                {
                    continue;
                }

                MeshHandle handle = factory.CreateMesh(data);
                MeshAsset asset = new MeshAsset(handle, data.VertexCount, data.IndexCount);
                c.Mesh = asset;
            }

            // if (c.TryGetMesh(out MeshAsset? m))
            if (c.Mesh.Handle.Handle.IsValid)
            {
                RenderCommand cmd = new(c.Mesh, _material, Matrix4x4.CreateTranslation((Vector3)c.Position));
                renderManager.Submit(cmd);
            }
        }
        // watch.Stop();
        // Logger.Info($"Rendering took: {watch.Elapsed.ToString()}");
    }

}
