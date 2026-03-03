namespace VoxelEngine.Core;

public readonly record struct Actor
{
    public readonly Entity Entity { get; init; }
    public readonly Scene Scene { get; init; }

    public Actor(Entity entity, Scene scene)
    {
        Entity = entity;
        Scene = scene;
    }
}