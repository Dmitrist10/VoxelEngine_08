namespace VoxelEngine.Core;

public record struct C_Hierarchy : IComponent
{
    // Parent-child relationship
    public Entity Parent; // Reference to parent entity (Entity.Null if root)
    public Entity FirstChild; // Reference to first child entity

    // Sibling linked list
    public Entity NextSibling; // Next sibling in parent's children list
    public Entity PreviousSibling; // Previous sibling (for fast removal)

    // Metadata
    public int Depth; // Distance from root (0 = root)
    public int ChildCount; // Number of direct children

    public C_Hierarchy()
    {
        Parent = Entity.Null;
        FirstChild = Entity.Null;
        NextSibling = Entity.Null;
        PreviousSibling = Entity.Null;
        Depth = 0;
        ChildCount = 0;
    }

    // Helper methods
    public bool IsRoot => Parent == Entity.Null;
    public bool HasChildren => FirstChild != Entity.Null;
}