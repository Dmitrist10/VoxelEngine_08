using VoxelEngine.Core;
using VoxelEngine.Graphics;

namespace VoxelEngine.Packages.Voxel;

public sealed class VoxelWorld : SceneGameService, IUpdatable, IRenderable
{

    // private readonly ChunksStorage _chunksStorage;
    // private readonly ChunksStreamer _chunksStreamer;
    // private readonly ChunksPhysics _chunksPhysics;
    // private readonly ChunksRenderer _chunksRenderer;
    private ChunksStorage _chunksStorage = null!;
    private ChunksStreamer _chunksStreamer = null!;
    private ChunksPhysics _chunksPhysics = null!;
    private ChunksRenderer _chunksRenderer = null!;

    private BlocksRegistrie _blocksRegistrie = null!;



    public VoxelWorld()
    {
        // _chunksStorage = new();
        // _chunksPhysics = new();

        // _chunksStreamer = new ChunksStreamer(_chunksStorage, scene.World);
        // _chunksRenderer = new ChunksRenderer(_chunksStorage);
    }

    public override void OnInitialized()
    {
        _chunksStorage = new();
        _chunksPhysics = new();

        _chunksStreamer = new ChunksStreamer(_chunksStorage, scene.World);

        _blocksRegistrie = new();
        ServiceContainer.Register<BlocksRegistrie>(_blocksRegistrie);

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

        _blocksRegistrie.Register(new BlockDescription("test", "STD:test", chunkTextures[0]));
        _blocksRegistrie.Register(new BlockDescription("Floor_1_overgrown", "STD:Floor_1_overgrown", chunkTextures[1]));
        _blocksRegistrie.Register(new BlockDescription("Floor_2", "STD:Floor_2", chunkTextures[2]));
        _blocksRegistrie.Register(new BlockDescription("Stone_01", "STD:Stone_01", chunkTextures[3]));
        _blocksRegistrie.Register(new BlockDescription("Bricks_01", "STD:Bricks_01", chunkTextures[4]));
        _blocksRegistrie.Register(new BlockDescription("Grass_01", "STD:Grass_01", chunkTextures[5]));
        _blocksRegistrie.Register(new BlockDescription("LuckyBlock_01", "STD:LuckyBlock_01", chunkTextures[6]));
        _blocksRegistrie.Register(new BlockDescription("Dirt_01", "STD:Dirt_01", chunkTextures[7]));

        var textureData = _blocksRegistrie.Build();
        var factory = ServiceContainer.Get<GraphicsContext>()!.Device.Factory;
        var textureHandle = factory.CreateTexture(textureData);

        _chunksRenderer = new ChunksRenderer(_chunksStorage, textureHandle);
    }


    public void OnUpdate()
    {
        _chunksStreamer.OnUpdate();
        // _chunksPhysics.OnUpdate();
    }
    public void OnRender()
    {
        _chunksRenderer.Render();
    }

    public void Dispose()
    {
        _chunksStorage.Dispose();
        _chunksStreamer.Dispose();
        // _chunksPhysics.Dispose();
        // _chunksRenderer.Dispose();
    }

}
