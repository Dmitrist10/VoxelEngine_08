using VoxelEngine.Core;

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
        _chunksRenderer = new ChunksRenderer(_chunksStorage);
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
