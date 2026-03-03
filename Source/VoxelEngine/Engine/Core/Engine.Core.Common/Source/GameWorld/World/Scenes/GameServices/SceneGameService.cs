namespace VoxelEngine.Core;

public abstract class SceneGameService : ISceneGameService
{
    protected Scene scene { get; private set; } = null!;

    public void SetUp(Scene scene)
    {
        this.scene = scene;
    }

    public virtual void OnInitialized() { }
    public virtual void OnShutdown() { }

}