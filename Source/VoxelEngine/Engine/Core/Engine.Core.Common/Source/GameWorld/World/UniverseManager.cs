namespace VoxelEngine.Core;

public sealed class UniverseManager
{
    private readonly List<Universe> _universes = new();

    public Universe CreateUniverse()
    {
        var universe = new Universe();
        _universes.Add(universe);
        return universe;
    }

    public void RemoveUniverse(Universe universe)
    {
        _universes.Remove(universe);
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