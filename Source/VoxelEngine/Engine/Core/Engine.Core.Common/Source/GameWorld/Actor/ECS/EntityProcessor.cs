using Arch.Core;

namespace VoxelEngine.Core;


public abstract class EntityProcessor : IEntityProcessor
{
    public Scene scene { get; private set; } = null!;
    public World world => scene.World;

    public void SetUp(Scene scene)
    {
        this.scene = scene;
    }

    public virtual void OnInitialize() { }
    public virtual void OnShutDown() { }
}