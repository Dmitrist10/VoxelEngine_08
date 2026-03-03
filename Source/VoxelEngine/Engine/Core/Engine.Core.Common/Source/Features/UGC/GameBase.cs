namespace VoxelEngine.Core.UGC;

public abstract class GameBase : IGame
{
    public virtual string Name => "Default";
    protected UniverseManager UniverseManager { get; private set; } = null!;

    public void SetUp(UniverseManager universeManager)
    {
        UniverseManager = universeManager;
    }

    public virtual void OnInitialize()
    {
    }

    public virtual void OnStartMatch()
    {
    }

    public virtual void OnUpdate()
    {
    }

    public virtual void OnShutdown()
    {
    }
}
