namespace VoxelEngine.Core;

internal sealed class EntityProcessorsRegistry
{

    private readonly Dictionary<Type, IEntityProcessor> _processors = new();
    private readonly List<IEntityProcessor> _allProcessors = new();

    private readonly List<IUpdatable> _allUpdatables = new();
    private readonly List<IFixedUpdatable> _allFixedUpdatables = new();
    private readonly List<IRenderable> _allRenderables = new();
    private readonly List<ITickable> _allTickables = new();

    private readonly Scene _scene;

    public EntityProcessorsRegistry(Scene scene)
    {
        _scene = scene;
    }

    public T AddProcessor<T>(T processor) where T : class, IEntityProcessor
    {
        _processors.Add(typeof(T), processor);
        _allProcessors.Add(processor);

        if (processor is IUpdatable updatable)
            _allUpdatables.Add(updatable);

        if (processor is IFixedUpdatable fixedUpdatable)
            _allFixedUpdatables.Add(fixedUpdatable);

        if (processor is IRenderable renderable)
            _allRenderables.Add(renderable);

        if (processor is ITickable tickable)
            _allTickables.Add(tickable);

        processor.SetUp(_scene);
        processor.OnInitialize();
        return processor;
    }
    [MethodImpl(AggressiveInlining)]
    public T? GetProcessor<T>() where T : class, IEntityProcessor
    {
        return _processors.TryGetValue(typeof(T), out var processor) ? processor as T : null;
    }
    [MethodImpl(AggressiveInlining)]
    public bool HasProcessor<T>() where T : class, IEntityProcessor
    {
        return _processors.ContainsKey(typeof(T));
    }
    [MethodImpl(AggressiveInlining)]
    public void RemoveProcessor<T>() where T : class, IEntityProcessor
    {
        if (_processors.TryGetValue(typeof(T), out var processor))
        {
            processor.OnShutDown();
            _processors.Remove(typeof(T));
            _allProcessors.Remove(processor);

            if (processor is IUpdatable updatable)
                _allUpdatables.Remove(updatable);

            if (processor is IFixedUpdatable fixedUpdatable)
                _allFixedUpdatables.Remove(fixedUpdatable);

            if (processor is IRenderable renderable)
                _allRenderables.Remove(renderable);

            if (processor is ITickable tickable)
                _allTickables.Remove(tickable);
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

    internal void OnRender()
    {
        foreach (var i in _allRenderables)
        {
            i.OnRender();
        }
    }

    internal void OnTick()
    {
        foreach (var i in _allTickables)
        {
            i.OnTick();
        }
    }
}