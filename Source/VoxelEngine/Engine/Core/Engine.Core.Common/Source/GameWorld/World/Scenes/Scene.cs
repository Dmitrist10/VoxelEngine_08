namespace VoxelEngine.Core;

public sealed partial class Scene
{
    private readonly World _world;
    public World World => _world;
    private readonly Universe _universe;
    public Universe Universe => _universe;

    // Registries
    private readonly ActorsRegistry _actorsRegistry;
    private readonly SceneGameServicesRegistry _servicesRegistry;
    private readonly EntityProcessorsRegistry _entityProcessorsRegistry;

    private readonly SceneCommandBuffer _commandBuffer;

    public Scene(Universe universe)
    {
        _world = World.Create();
        _universe = universe;

        _actorsRegistry = new ActorsRegistry(this, _world);
        _servicesRegistry = new SceneGameServicesRegistry(this);
        _entityProcessorsRegistry = new EntityProcessorsRegistry(this);

        _commandBuffer = new SceneCommandBuffer();
    }

}
