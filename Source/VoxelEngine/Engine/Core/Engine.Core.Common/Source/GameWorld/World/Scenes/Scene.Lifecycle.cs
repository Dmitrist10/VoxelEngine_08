namespace VoxelEngine.Core;

public sealed partial class Scene
{


    internal void OnUpdate()
    {
        _servicesRegistry.OnUpdate();
        _actorsRegistry.OnUpdate();
    }
    internal void OnFixedUpdate()
    {
        _servicesRegistry.OnFixedUpdate();
        _actorsRegistry.OnFixedUpdate();
    }

    internal void OnTick()
    {
        // _actorsRegistry.OnTick();
    }
    internal void OnRender()
    {
        // _actorsRegistry.OnRender();
    }


}
