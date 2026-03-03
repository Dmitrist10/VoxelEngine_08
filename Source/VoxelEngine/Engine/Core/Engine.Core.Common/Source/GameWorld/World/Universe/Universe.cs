namespace VoxelEngine.Core;

public sealed partial class Universe
{
    private readonly List<Scene> _scenes = new();
    private readonly UniverseGameServicesRegistry _servicesRegistry;

    public Universe()
    {
        _servicesRegistry = new UniverseGameServicesRegistry(this);
    }

}