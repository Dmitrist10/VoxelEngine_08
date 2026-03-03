namespace VoxelEngine.Core;

public abstract class UniverseGameService : IUniverseGameService
{
    protected Universe Universe { get; private set; } = null!;

    public void SetUp(Universe universe)
    {
        Universe = universe;
    }

    public virtual void OnInitialized() { }
    public virtual void OnShutdown() { }

}