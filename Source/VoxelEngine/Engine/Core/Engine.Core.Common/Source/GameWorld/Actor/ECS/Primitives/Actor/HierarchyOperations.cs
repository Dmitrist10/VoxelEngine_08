using System.Numerics;
using Arch.Core;

// ReSharper disable MemberCanBePrivate.Global

namespace VoxelEngine.Core;

/// <summary>
/// Provides low-level ECS operations for managing entity hierarchy relationships.
/// Handles linking/unlinking of C_Hierarchy components and optional transform preservation.
/// </summary>
public static class HierarchyOperations
{
    /// <summary>
    /// Sets the parent of a child entity.
    /// </summary>
    /// <param name="world">The ECS world.</param>
    /// <param name="child">The child entity.</param>
    /// <param name="parent">The new parent entity. Use Entity.Null to detach.</param>
    /// <param name="worldPositionStays">If true, preserves the child's world position/rotation/scale.</param>
    public static void SetParent(in World world, in Entity child, in Entity parent, bool worldPositionStays = true)
    {
        ref var childHierarchy = ref world.Get<C_Hierarchy>(child);
        ref var childTransform = ref world.Get<C_Transform>(child);

        SetParent(world, child, ref childHierarchy, ref childTransform, parent, worldPositionStays);
    }

    /// <summary>
    /// Optimized generic overload for setting parent when components are already available.
    /// </summary>
    public static void SetParent(in World world, in Entity child, ref C_Hierarchy childHierarchy,
        ref C_Transform childTransform, in Entity parent, bool worldPositionStays = true)
    {
        if (childHierarchy.Parent == parent)
            return;

        // Ensure we are not parenting to ourselves or a descendant (cycle prevention)
        if (parent != Entity.Null && (child == parent || IsDescendant(world, parent, child)))
        {
            // Logging would be good here, but for now just return to prevent corruption.
            return;
        }

        // 1. Capture World State if needed
        Vector3 previousWorldPos = childTransform.WorldPosition;
        Quaternion previousWorldRot = childTransform.WorldRotation;
        Vector3 previousWorldScale = childTransform.WorldScale;

        // 2. Detach from current parent
        if (childHierarchy.Parent != Entity.Null && world.IsAlive(childHierarchy.Parent))
        {
            RemoveFromParentList(world, childHierarchy.Parent, ref childHierarchy);
        }

        // 3. Attach to new parent
        childHierarchy.Parent = parent;

        if (parent != Entity.Null && world.IsAlive(parent))
        {
            AddToParentList(world, parent, child, ref childHierarchy);

            // Update Depth
            var parentHierarchy = world.Get<C_Hierarchy>(parent);
            UpdateDepthRecursive(world, child, parentHierarchy.Depth + 1);
        }
        else
        {
            childHierarchy.Parent = Entity.Null;
            UpdateDepthRecursive(world, child, 0);
        }

        // 4. Update Transform
        if (worldPositionStays)
        {
            if (parent == Entity.Null)
            {
                // Unparenting: Local becomes World
                childTransform.LocalPosition = previousWorldPos;
                childTransform.LocalRotation = previousWorldRot;
                childTransform.LocalScale = previousWorldScale;
            }
            else
            {
                // Parenting: Calculate Local from World relative to new Parent
                ref var parentTransform = ref world.Get<C_Transform>(parent);

                // Inverse Transform Point/Direction logic
                var inverseParentRot = Quaternion.Inverse(parentTransform.WorldRotation);

                // Scale
                Vector3 parentScale = parentTransform.WorldScale;
                // Avoid divide by zero
                parentScale = new Vector3(
                    System.MathF.Abs(parentScale.X) < 1e-6f ? 1f : parentScale.X,
                    System.MathF.Abs(parentScale.Y) < 1e-6f ? 1f : parentScale.Y,
                    System.MathF.Abs(parentScale.Z) < 1e-6f ? 1f : parentScale.Z
                );

                childTransform.LocalScale = previousWorldScale / parentScale;

                // Rotation
                childTransform.LocalRotation = inverseParentRot * previousWorldRot;

                // Position
                Vector3 relPos = previousWorldPos - parentTransform.WorldPosition;
                childTransform.LocalPosition = Vector3.Transform(relPos, inverseParentRot);
                childTransform.LocalPosition /= parentScale; // Apply inverse scale
            }
        }

        childTransform.IsDirty = true;
    }

    /// <summary>
    /// Adds a child to the parent entity. Wraps SetParent.
    /// </summary>
    public static void AddChild(in World world, in Entity parent, in Entity child, bool worldPositionStays = true)
    {
        SetParent(world, child, parent, worldPositionStays);
    }

    /// <summary>
    /// Removes a child from its parent (effectively unparenting it).
    /// </summary>
    public static void RemoveChild(in World world, in Entity parent, in Entity child, bool worldPositionStays = true)
    {
        // Only unparent if it's actually our child
        if (world.IsAlive(child))
        {
            var hierarchy = world.Get<C_Hierarchy>(child);
            if (hierarchy.Parent == parent)
            {
                SetParent(world, child, Entity.Null, worldPositionStays);
            }
        }
    }

    /// <summary>
    /// Detaches the entity from its parent.
    /// </summary>
    public static void DetachFromParent(in World world, in Entity child, bool worldPositionStays = true)
    {
        SetParent(world, child, Entity.Null, worldPositionStays);
    }

    /// <summary>
    /// Gets all immediate children of a parent entity.
    /// </summary>
    public static List<Entity> GetChildren(in World world, in Entity parent)
    {
        var list = new List<Entity>();
        if (!world.IsAlive(parent)) return list;

        ref var parentHierarchy = ref world.Get<C_Hierarchy>(parent);
        var current = parentHierarchy.FirstChild;

        while (current != Entity.Null && world.IsAlive(current))
        {
            list.Add(current);
            current = world.Get<C_Hierarchy>(current).NextSibling;
        }

        return list;
    }

    /// <summary>
    /// Gets the parent entity of a child.
    /// </summary>
    public static Entity GetParent(in World world, in Entity child)
    {
        if (!world.IsAlive(child)) return Entity.Null;
        return world.Get<C_Hierarchy>(child).Parent;
    }

    /// <summary>
    /// Gets a child entity by index. Returns Entity.Null if index is out of range.
    /// </summary>
    public static Entity GetChild(in World world, in Entity parent, int index)
    {
        if (!world.IsAlive(parent)) return Entity.Null;

        ref var hierarchy = ref world.Get<C_Hierarchy>(parent);
        Entity current = hierarchy.FirstChild;
        int currentIndex = 0;

        while (current != Entity.Null)
        {
            if (currentIndex == index) return current;

            ref var childHierarchy = ref world.Get<C_Hierarchy>(current);
            current = childHierarchy.NextSibling;
            currentIndex++;
        }

        return Entity.Null;
    }

    /// <summary>
    /// Finds a child by name (recursive or direct? Usually direct in simple FindChild).
    /// This implementation is DIRECT child only.
    /// </summary>
    public static Entity FindChild(in World world, in Entity parent, string name)
    {
        if (!world.IsAlive(parent)) return Entity.Null;

        ref var hierarchy = ref world.Get<C_Hierarchy>(parent);
        Entity current = hierarchy.FirstChild;

        while (current != Entity.Null)
        {
            if (world.Get<C_Actor>(current).Name == name)
            {
                return current;
            }

            current = world.Get<C_Hierarchy>(current).NextSibling;
        }

        return Entity.Null;
    }

    /// <summary>
    /// Checks if potentialDescendant is a descendant (child, grandchild, etc.) of potentialAncestor.
    /// </summary>
    public static bool IsDescendant(in World world, in Entity potentialDescendant, in Entity potentialAncestor)
    {
        var current = potentialDescendant;
        while (current != Entity.Null && world.IsAlive(current))
        {
            var parent = world.Get<C_Hierarchy>(current).Parent;
            if (parent == potentialAncestor) return true;
            if (parent == current) break; // Safety break for loops
            current = parent;
        }

        return false;
    }

    /// <summary>
    /// Gets the root of the hierarchy for the given entity.
    /// </summary>
    public static Entity GetRoot(in World world, in Entity entity)
    {
        var current = entity;
        while (true)
        {
            if (!world.IsAlive(current)) return Entity.Null;
            var parent = world.Get<C_Hierarchy>(current).Parent;
            if (parent == Entity.Null) return current;
            current = parent;
        }
    }

    // --- Internal Linked List Logic ---

    private static void AddToParentList(in World world, in Entity parent, in Entity child, ref C_Hierarchy childHierarchy)
    {
        ref var parentHierarchy = ref world.Get<C_Hierarchy>(parent);

        if (parentHierarchy.FirstChild == Entity.Null)
        {
            parentHierarchy.FirstChild = child;
            childHierarchy.PreviousSibling = Entity.Null;
            childHierarchy.NextSibling = Entity.Null;
        }
        else
        {
            // Prepend for O(1) performance
            var oldFirst = parentHierarchy.FirstChild;
            parentHierarchy.FirstChild = child;
            childHierarchy.NextSibling = oldFirst;
            childHierarchy.PreviousSibling = Entity.Null;

            if (oldFirst != Entity.Null && world.IsAlive(oldFirst))
            {
                ref var oldFirstHierarchy = ref world.Get<C_Hierarchy>(oldFirst);
                oldFirstHierarchy.PreviousSibling = child;
            }
        }

        parentHierarchy.ChildCount++;
    }

    private static void RemoveFromParentList(in World world, in Entity parent, ref C_Hierarchy childHierarchy)
    {
        ref var parentHierarchy = ref world.Get<C_Hierarchy>(parent);

        var prev = childHierarchy.PreviousSibling;
        var next = childHierarchy.NextSibling;

        if (prev != Entity.Null)
        {
            ref var prevHierarchy = ref world.Get<C_Hierarchy>(prev);
            prevHierarchy.NextSibling = next;
        }
        else
        {
            // We are the first child
            parentHierarchy.FirstChild = next;
        }

        if (next != Entity.Null)
        {
            ref var nextHierarchy = ref world.Get<C_Hierarchy>(next);
            nextHierarchy.PreviousSibling = prev;
        }

        childHierarchy.PreviousSibling = Entity.Null;
        childHierarchy.NextSibling = Entity.Null;
        parentHierarchy.ChildCount--;
    }

    private static void UpdateDepthRecursive(in World world, in Entity entity, int depth)
    {
        ref var hierarchy = ref world.Get<C_Hierarchy>(entity);
        hierarchy.Depth = depth;

        var child = hierarchy.FirstChild;
        while (child != Entity.Null)
        {
            UpdateDepthRecursive(world, child, depth + 1);
            child = world.Get<C_Hierarchy>(child).NextSibling;
        }
    }
}
