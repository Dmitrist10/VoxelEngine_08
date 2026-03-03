using System.Diagnostics.CodeAnalysis;

namespace VoxelEngine.Core;

public sealed partial class Scene
{

    // Actors
    public Actor CreateActor() => _actorsRegistry.CreateActor();
    public Entity CreateEntity() => _actorsRegistry.CreateEntity();

    // public void Destroy(in Actor actor) => _commandBuffer.Enqueue(() => _actorsRegistry.Destroy(actor));
    // public void Destroy(in Entity entity) => _commandBuffer.Enqueue(() => _actorsRegistry.Destroy(entity));


    // Behavior
    public T AddBehavior<T>(in Actor actor) where T : class, IBehavior, new()
    {
        return _actorsRegistry.AddBehavior(actor, new T());
    }
    public T AddBehavior<T>(in Actor actor, T instance) where T : class, IBehavior
    {
        return _actorsRegistry.AddBehavior(actor, instance);
    }
    public T? GetBehavior<T>(in Actor actor) where T : class, IBehavior
    {
        return _actorsRegistry.GetBehavior<T>(actor);
    }
    public bool TryGetBehavior<T>(in Actor actor, [NotNullWhen(true)] out T? behavior) where T : class, IBehavior
    {
        behavior = _actorsRegistry.GetBehavior<T>(actor);
        return behavior != null;
    }
    public bool HasBehavior<T>(in Actor actor) where T : class, IBehavior
    {
        return _actorsRegistry.HasBehavior<T>(actor);
    }
    public void RemoveBehavior<T>(in Actor actor) where T : class, IBehavior
    {
        _actorsRegistry.RemoveBehavior<T>(actor);
    }


    // Game Services
    public T AddService<T>() where T : class, ISceneGameService, new() => _servicesRegistry.AddService(new T());
    public T AddService<T>(T service) where T : class, ISceneGameService => _servicesRegistry.AddService(service);
    public T? GetService<T>() where T : class, ISceneGameService => _servicesRegistry.GetService<T>();
    public bool TryGetService<T>([NotNullWhen(true)] out T? service) where T : class, ISceneGameService
    {
        service = _servicesRegistry.GetService<T>();
        return service != null;
    }
    public bool HasService<T>() where T : class, ISceneGameService => _servicesRegistry.HasService<T>();
    public void RemoveService<T>() where T : class, ISceneGameService => _servicesRegistry.RemoveService<T>();

    // Entity Processors
    public T AddProcessor<T>() where T : class, IEntityProcessor, new() => _entityProcessorsRegistry.AddProcessor(new T());
    public T AddProcessor<T>(T processor) where T : class, IEntityProcessor => _entityProcessorsRegistry.AddProcessor(processor);
    public T? GetProcessor<T>() where T : class, IEntityProcessor => _entityProcessorsRegistry.GetProcessor<T>();
    public bool TryGetProcessor<T>([NotNullWhen(true)] out T? processor) where T : class, IEntityProcessor
    {
        processor = _entityProcessorsRegistry.GetProcessor<T>();
        return processor != null;
    }
    public bool HasProcessor<T>() where T : class, IEntityProcessor => _entityProcessorsRegistry.HasProcessor<T>();
    public void RemoveProcessor<T>() where T : class, IEntityProcessor => _entityProcessorsRegistry.RemoveProcessor<T>();

}
