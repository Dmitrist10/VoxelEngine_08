namespace VoxelEngine.Core;

public sealed partial class Scene
{
    public World World { get; }
    public Universe Universe { get; }

    // Registries
    private readonly ActorsRegistry _actorsRegistry;
    private readonly SceneGameServicesRegistry _servicesRegistry;
    private readonly EntityProcessorsRegistry _entityProcessorsRegistry;

    private readonly SceneCommandBuffer _commandBuffer;

    public Scene(Universe universe)
    {
        World = World.Create();
        Universe = universe;

        _actorsRegistry = new ActorsRegistry(this, World);
        _servicesRegistry = new SceneGameServicesRegistry(this);
        _entityProcessorsRegistry = new EntityProcessorsRegistry(this);

        _commandBuffer = new SceneCommandBuffer();

        AddProcessor(new EP_Transform());
    }

}
