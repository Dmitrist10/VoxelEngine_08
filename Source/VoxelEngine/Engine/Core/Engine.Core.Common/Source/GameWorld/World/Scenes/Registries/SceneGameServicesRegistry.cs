namespace VoxelEngine.Core;

internal sealed class SceneGameServicesRegistry
{
    private readonly Dictionary<Type, ISceneGameService> _services = new();
    private readonly List<ISceneGameService> _allServices = new();

    private readonly List<IUpdatable> _allUpdatables = new();
    private readonly List<IFixedUpdatable> _allFixedUpdatables = new();
    private readonly List<IRenderable> _allRenderables = new();
    private readonly List<ITickable> _allTickable = new();

    private readonly Scene _scene;

    public SceneGameServicesRegistry(Scene scene)
    {
        _scene = scene;
    }

    public T AddService<T>(T service) where T : class, ISceneGameService
    {
        _services.Add(typeof(T), service);
        _allServices.Add(service);

        if (service is IUpdatable updatable)
            _allUpdatables.Add(updatable);

        if (service is IFixedUpdatable fixedUpdatable)
            _allFixedUpdatables.Add(fixedUpdatable);

        if (service is IRenderable renderable)
            _allRenderables.Add(renderable);

        if (service is ITickable tickable)
            _allTickable.Add(tickable);

        service.SetUp(_scene);
        service.OnInitialized();
        return service;
    }
    [MethodImpl(AggressiveInlining)]
    public T? GetService<T>() where T : class, ISceneGameService
    {
        return _services.TryGetValue(typeof(T), out var service) ? service as T : null;
    }
    [MethodImpl(AggressiveInlining)]
    public bool HasService<T>() where T : class, ISceneGameService
    {
        return _services.ContainsKey(typeof(T));
    }
    [MethodImpl(AggressiveInlining)]
    public void RemoveService<T>() where T : class, ISceneGameService
    {
        if (_services.TryGetValue(typeof(T), out var service))
        {
            service.OnShutdown();
            _services.Remove(typeof(T));
            _allServices.Remove(service);

            if (service is IUpdatable updatable)
                _allUpdatables.Remove(updatable);

            if (service is IFixedUpdatable fixedUpdatable)
                _allFixedUpdatables.Remove(fixedUpdatable);

            if (service is IRenderable renderable)
                _allRenderables.Remove(renderable);

            if (service is ITickable t)
                _allTickable.Remove(t);
        }
    }

    internal void OnUpdate()
    {
        foreach (var i in _allUpdatables)
        {
            i.OnUpdate();
        }
    }
    internal void OnFixedUpdate()
    {
        foreach (var i in _allFixedUpdatables)
        {
            i.OnFixedUpdate();
        }
    }
    internal void OnTick()
    {
        foreach (var i in _allTickable)
        {
            i.OnTick();
        }
    }
    internal void OnRender()
    {
        foreach (var i in _allRenderables)
        {
            i.OnRender();
        }
    }

}