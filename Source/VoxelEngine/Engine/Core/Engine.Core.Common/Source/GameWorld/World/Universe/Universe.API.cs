using System.Diagnostics.CodeAnalysis;
using VoxelEngine.Diagnostics;

namespace VoxelEngine.Core;

public sealed partial class Universe
{

    public Scene CreateScene()
    {
        var scene = new Scene(this);
        _scenes.Add(scene);
        Logger.ExtraInfo("A new Scene was created");
        UniverseManager.instance.CallSceneCreated(scene);
        return scene;
    }

    public void RemoveScene(Scene scene)
    {
        _scenes.Remove(scene);
        Logger.ExtraInfo("A Scene was destroyed");
        UniverseManager.instance.CallSceneDestroyed(scene);
    }


    public bool TryGetService<T>([NotNullWhen(true)] out T? service) where T : class, IUniverseGameService
    {
        service = _servicesRegistry.GetService<T>();
        return service != null;
    }
    public T? GetService<T>() where T : class, IUniverseGameService
    {
        return _servicesRegistry.GetService<T>();
    }
    public T AddService<T>() where T : class, IUniverseGameService, new()
    {
        return _servicesRegistry.AddService(new T());
    }
    public T AddService<T>(T service) where T : class, IUniverseGameService
    {
        return _servicesRegistry.AddService(service);
    }
    public void RemoveService<T>() where T : class, IUniverseGameService
    {
        _servicesRegistry.RemoveService<T>();
    }
    public bool HasService<T>() where T : class, IUniverseGameService
    {
        return _servicesRegistry.HasService<T>();
    }

}