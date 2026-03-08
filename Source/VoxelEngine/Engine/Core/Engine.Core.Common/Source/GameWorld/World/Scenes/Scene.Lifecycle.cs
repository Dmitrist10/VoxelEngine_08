namespace VoxelEngine.Core;

public sealed partial class Scene
{


    internal void OnUpdate()
    {
        _servicesRegistry.OnUpdate();
        _actorsRegistry.OnUpdate();
        _entityProcessorsRegistry.OnUpdate();
    }
    internal void OnFixedUpdate()
    {
        _servicesRegistry.OnFixedUpdate();
        _actorsRegistry.OnFixedUpdate();
        _entityProcessorsRegistry.OnFixedUpdate();
    }

    internal void OnTick()
    {
        _entityProcessorsRegistry.OnTick();
    }
    internal void OnRender()
    {
        _servicesRegistry.OnRender();
        _entityProcessorsRegistry.OnRender();
        // _actorsRegistry.OnRender();
    }


}
