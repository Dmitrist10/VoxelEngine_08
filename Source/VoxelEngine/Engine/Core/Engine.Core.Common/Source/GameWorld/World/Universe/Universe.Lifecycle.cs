namespace VoxelEngine.Core;

public sealed partial class Universe
{

    internal void OnUpdate()
    {
        // _servicesRegistry.OnUpdate();

        foreach (var scene in _scenes)
            scene.OnUpdate();
    }
    internal void OnFixedUpdate()
    {
        // _servicesRegistry.OnUpdate();

        foreach (var scene in _scenes)
            scene.OnFixedUpdate();
    }
    internal void OnTick()
    {
        // _servicesRegistry.OnUpdate();

        foreach (var scene in _scenes)
            scene.OnTick();
    }
    internal void OnRender()
    {
        // _servicesRegistry.OnUpdate();

        foreach (var scene in _scenes)
            scene.OnRender();
    }

}