namespace VoxelEngine.Core;

public record struct C_Actor : IComponent
{
    // public Guid Scene;
    public string Name = "Actor";
    public Guid ID;

    public C_Actor()
    {
        ID = Guid.NewGuid();
    }
    public C_Actor(Guid id)
    {
        ID = id;
    }

}
