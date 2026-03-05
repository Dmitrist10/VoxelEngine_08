using VoxelEngine.Core;

namespace VoxelEngine.Packages.Voxel;

public sealed class VoxelWorld : SceneGameService, IUpdatable, IRenderable
{

    private readonly ChunksStorage _chunksStorage = new();
    private readonly ChunksStreamer _chunksStreamer;
    private readonly ChunksPhysics _chunksPhysics = new();
    private readonly ChunksRenderer _chunksRenderer;

    public VoxelWorld()
    {
        _chunksStreamer = new ChunksStreamer(_chunksStorage, scene.World);
        _chunksRenderer = new ChunksRenderer(_chunksStorage);
    }

    public override void OnInitialized()
    {
    }

    public void OnRender()
    {
        _chunksRenderer.Render();
    }

    public void OnUpdate()
    {
        _chunksStreamer.OnUpdate();
        // _chunksPhysics.OnUpdate();
    }

    public void Dispose()
    {
        _chunksStorage.Dispose();
        _chunksStreamer.Dispose();
        // _chunksPhysics.Dispose();
        // _chunksRenderer.Dispose();
    }

}
