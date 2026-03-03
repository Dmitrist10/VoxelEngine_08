using VoxelEngine.Diagnostics;

namespace VoxelEngine.Core;

public static class ServiceContainer
{
    private static readonly Dictionary<Type, object> Services;

    static ServiceContainer()
    {
        Services = new Dictionary<Type, object>();
    }

    public static void Register<T>(T service)
    {
        if (service == null)
        {
            Logger.Error($"[ServiceContainer] Service {typeof(T)} is null.");
            return;
        }

        Logger.ExtraInfo($"[ServiceContainer] Registering service {typeof(T)}");
        Services[typeof(T)] = service;
    }

    public static T? Get<T>()
    {
        if (!Services.ContainsKey(typeof(T)))
        {
            Logger.Error($"[ServiceContainer] Service {typeof(T)} is not registered.");
            return default;
        }

        return (T)Services[typeof(T)];
    }

    public static void Unregister<T>()
    {
        if (Services.ContainsKey(typeof(T)))
        {
            Services.Remove(typeof(T));
        }
    }

    public static bool TryGet<T>(out T? service)
    {
        if (Services.ContainsKey(typeof(T)))
        {
            service = (T)Services[typeof(T)];
            return true;
        }

        service = default;
        return false;
    }

    public static bool IsRegistered<T>()
    {
        return Services.ContainsKey(typeof(T));
    }

    public static void Clear()
    {
        Logger.Info("[ServiceContainer] Clearing all registered services");
        Services.Clear();
    }

}