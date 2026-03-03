namespace VoxelEngine.Core;

internal sealed class UniverseGameServicesRegistry
{
    private readonly Dictionary<Type, IUniverseGameService> _services = new();

    private readonly Universe _universe;

    public UniverseGameServicesRegistry(Universe universe)
    {
        _universe = universe;
    }

    public T AddService<T>(T service) where T : class, IUniverseGameService
    {
        _services.Add(typeof(T), service);
        service.SetUp(_universe);
        service.OnInitialized();
        return service;
    }
    [MethodImpl(AggressiveInlining)]
    public T? GetService<T>() where T : class, IUniverseGameService
    {
        return _services.TryGetValue(typeof(T), out var service) ? service as T : null;
    }
    [MethodImpl(AggressiveInlining)]
    public bool HasService<T>() where T : class, IUniverseGameService
    {
        return _services.ContainsKey(typeof(T));
    }
    [MethodImpl(AggressiveInlining)]
    public void RemoveService<T>() where T : class, IUniverseGameService
    {
        if (_services.TryGetValue(typeof(T), out var service))
        {
            service.OnShutdown();
            _services.Remove(typeof(T));
        }
    }

    // internal void OnUpdate()
    // {
    //     foreach (var service in _services.Values)
    //         service.OnUpdate();
    // }
}