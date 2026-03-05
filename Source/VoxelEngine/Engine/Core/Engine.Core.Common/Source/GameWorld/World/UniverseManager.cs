using VoxelEngine.Diagnostics;

namespace VoxelEngine.Core;

public sealed class UniverseManager : Singleton<UniverseManager>
{
    private readonly List<Universe> _universes = new();

    public Universe CreateUniverse()
    {
        var universe = new Universe();
        _universes.Add(universe);
        Logger.ExtraInfo("A new Universe was created");
        OnUniverseCreated?.Invoke(universe);
        return universe;
    }

    public event Action<Universe>? OnUniverseCreated;
    public event Action<Universe>? OnUniverseDestroyed;
    public event Action<Scene>? OnSceneCreated;
    public event Action<Scene>? OnSceneDestroyed;

    public void CallSceneCreated(Scene s) => OnSceneCreated?.Invoke(s);
    public void CallSceneDestroyed(Scene s) => OnSceneDestroyed?.Invoke(s);

    public void RemoveUniverse(Universe universe)
    {
        _universes.Remove(universe);
        Logger.ExtraInfo("A Universe was destroyed");
        OnUniverseDestroyed?.Invoke(universe);
    }

    public void Update()
    {
        foreach (var universe in _universes)
        {
            universe.OnUpdate();
        }
    }
    public void Render()
    {
        foreach (var universe in _universes)
        {
            universe.OnRender();
        }
    }
    public void FixedUpdate()
    {
        foreach (var universe in _universes)
        {
            universe.OnFixedUpdate();
        }
    }
    public void Tick()
    {
        foreach (var universe in _universes)
        {
            universe.OnTick();
        }
    }
}