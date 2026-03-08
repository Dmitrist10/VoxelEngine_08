using VoxelEngine.Core;

namespace VoxelEngine.Packages.Voxel;

public sealed class VoxelWorld : SceneGameService, IUpdatable, IRenderable, ITickable
{

    private ChunksRenderer? _chunksRenderer;
    private ChunksStreamer? _chunksStreamer;
    public readonly ChunksStorage ChunksStorage;

    public VoxelWorld()
    {
        // _chunksRenderer = new();
        // _chunksStreamer = new();
        ChunksStorage = new();
    }

    public override void OnInitialized()
    {
        _chunksRenderer = new(this, scene.World);
        _chunksStreamer = new(this, scene.World);
    }

    public void OnRender()
    {
        _chunksRenderer!.OnRender();
    }

    public void OnTick()
    {
        _chunksStreamer!.OnTick();
    }

    public void OnUpdate()
    {
        _chunksStreamer!.OnUpdate();
    }

}
