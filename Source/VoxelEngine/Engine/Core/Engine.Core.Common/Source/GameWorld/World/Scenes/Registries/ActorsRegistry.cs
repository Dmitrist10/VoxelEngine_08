namespace VoxelEngine.Core;

internal sealed class ActorsRegistry
{
    private readonly Scene _scene;
    private readonly World _world;

    private readonly Dictionary<Entity, Dictionary<Type, IBehavior>> _behaviors = new();
    private readonly List<IBehavior> _allBehaviors = new();

    private readonly List<IUpdatable> _allUpdatables = new();
    // private readonly List<IFixedUpdatable> _allFixedUpdatables = new();
    // private readonly List<ITickable> _allTicks = new();
    // private readonly List<IRenderable> _allRenderables = new();

    public uint EntitysCount { get; private set; } = 0;

    public ActorsRegistry(Scene scene, World world)
    {
        _scene = scene;
        _world = world;
    }

    [MethodImpl(AggressiveInlining)]
    public Actor CreateActor() => new Actor(CreateEntity(), _scene);

    [MethodImpl(AggressiveInlining)]
    internal Entity CreateEntity()
    {
        var entity = _world.Create(new C_Actor(), new C_Transform(), new C_Hierarchy(), new C_WorldTransformMatrix());
        EntitysCount++;
        return entity;
    }

    [MethodImpl(AggressiveInlining)]
    public void Destroy(in Actor actor)
    {
        Destroy(actor.Entity);
    }
    [MethodImpl(AggressiveInlining)]
    public void Destroy(in Entity entity)
    {
        DestroyAllBehaviorsFrom(in entity);
        EntitysCount--;
        _world.Destroy(entity);
    }

    [MethodImpl(AggressiveInlining)]
    private void DestroyAllBehaviorsFrom(in Entity entity)
    {
        if (_behaviors.TryGetValue(entity, out var behaviors))
        {
            foreach (var behavior in behaviors.Values)
            {
                behavior.Invalidate();
                behavior.OnDestroy();
            }
            _behaviors.Remove(entity);
        }
    }

    [MethodImpl(AggressiveInlining)]
    public void DestroyAllBehaviors()
    {
        foreach (var behavior in _allBehaviors)
        {
            behavior.OnDestroy();
        }

        _allUpdatables.Clear();

        _behaviors.Clear();
        _allBehaviors.Clear();
    }

    [MethodImpl(AggressiveInlining)]
    public T AddBehavior<T>(in Actor actor) where T : class, IBehavior, new()
    {
        return AddBehavior(actor, new T());
    }
    [MethodImpl(AggressiveInlining)]
    public T AddBehavior<T>(in Actor actor, T instance) where T : class, IBehavior
    {
        if (_behaviors.TryGetValue(actor.Entity, out var behaviors))
            behaviors.Add(typeof(T), instance);
        else
        {
            behaviors = new Dictionary<Type, IBehavior>
            {
                { typeof(T), instance }
            };
            _behaviors.Add(actor.Entity, behaviors);
        }

        _allBehaviors.Add(instance);

        if (instance is IUpdatable updatable)
            _allUpdatables.Add(updatable);

        // if (instance is IFixedUpdatable fixedUpdatable)
        //     _allFixedUpdatables.Add(fixedUpdatable);

        // if (instance is ITickable tickable)
        //     _allTicks.Add(tickable);

        // if (instance is IRenderable renderable)
        //     _allRenderables.Add(renderable);

        instance.OnAwake();
        return instance;
    }

    [MethodImpl(AggressiveInlining)]
    public T? GetBehavior<T>(in Actor actor) where T : class, IBehavior
    {
        if (_behaviors.TryGetValue(actor.Entity, out var behaviors))
            if (behaviors.TryGetValue(typeof(T), out var behavior))
                return (T)behavior;

        return null;
    }
    [MethodImpl(AggressiveInlining)]
    public bool HasBehavior<T>(in Actor actor) where T : class, IBehavior
    {
        if (_behaviors.TryGetValue(actor.Entity, out var behaviors))
            return behaviors.ContainsKey(typeof(T));

        return false;
    }

    [MethodImpl(AggressiveInlining)]
    public void RemoveBehavior<T>(in Actor actor) where T : class, IBehavior
    {
        if (_behaviors.TryGetValue(actor.Entity, out var behaviors))
        {
            if (behaviors.TryGetValue(typeof(T), out var behavior))
            {
                behavior.Invalidate();
                behavior.OnDestroy();
                behaviors.Remove(typeof(T));
                _allBehaviors.Remove(behavior);

                if (behavior is IUpdatable updatable)
                    _allUpdatables.Remove(updatable);
            }
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
        // foreach (var i in _allFixedUpdatables)
        // {
        //     i.OnFixedUpdate();
        // }
    }
}