using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace VoxelEngine.Core;

public readonly record struct Actor
{
    public readonly Entity Entity { get; init; }
    public readonly Scene Scene { get; init; }
    public readonly World World => Scene.World;

    public Actor(Entity entity, Scene scene)
    {
        Entity = entity;
        Scene = scene;
    }

    [MethodImpl(AggressiveInlining)]
    public void AddComponent<TComponent>(in TComponent component) where TComponent : struct
    {
        Scene.World.Add(Entity, component);
    }

    [MethodImpl(AggressiveInlining)]
    public void AddComponent<TComponent>() where TComponent : struct
    {
        Scene.World.Add<TComponent>(Entity);
    }

    [MethodImpl(AggressiveInlining)]
    public bool TryGetComponent<T>([NotNullWhen(true)] out T component) where T : struct
    {
        return Scene.World.TryGet<T>(Entity, out component);
    }

    public delegate void ComponentModifier<T>(ref T component);

    [MethodImpl(AggressiveInlining)]
    public void ModifyComponent<T>(ComponentModifier<T> modifier) where T : struct
    {
        modifier(ref Scene.World.Get<T>(Entity));
    }

    // [MethodImpl(AggressiveInlining)]
    // public bool TryGetComponent<TComponent>(out TComponent component) where TComponent : unmanaged, IComponent
    // {
    //     component = Scene.World.Get<TComponent>(Entity);
    //     return component != null;
    // }
    [MethodImpl(AggressiveInlining)]
    public ref TComponent GetComponent<TComponent>() where TComponent : struct
    {
        return ref Scene.World.Get<TComponent>(Entity);
    }

    [MethodImpl(AggressiveInlining)]
    public void RemoveComponent<TComponent>() where TComponent : struct
    {
        Scene.World.Remove<TComponent>(Entity);
    }

    [MethodImpl(AggressiveInlining)]
    public bool HasComponent<TComponent>() where TComponent : struct
    {
        return Scene.World.Has<TComponent>(Entity);
    }


    #region Local Transform

    public Vector3 LocalPosition
    {
        [MethodImpl(AggressiveInlining)]
        get => World.Get<C_Transform>(Entity).LocalPosition;
        [MethodImpl(AggressiveInlining)]
        set
        {
            ref var transform = ref World.Get<C_Transform>(Entity);
            transform.LocalPosition = value;
            transform.IsDirty = true;
        }
    }

    public Quaternion LocalRotation
    {
        [MethodImpl(AggressiveInlining)]
        get => World.Get<C_Transform>(Entity).LocalRotation;
        [MethodImpl(AggressiveInlining)]
        set
        {
            ref var transform = ref World.Get<C_Transform>(Entity);
            transform.LocalRotation = value;
            transform.IsDirty = true;
        }
    }

    public Vector3 LocalScale
    {
        [MethodImpl(AggressiveInlining)]
        get => World.Get<C_Transform>(Entity).LocalScale;
        [MethodImpl(AggressiveInlining)]
        set
        {
            ref var transform = ref World.Get<C_Transform>(Entity);
            transform.LocalScale = value;
            transform.IsDirty = true;
        }
    }

    #endregion


    #region World Transform

    public Vector3 Forward => Vector3.Transform(Vector3.UnitZ, Rotation);
    public Vector3 Right => Vector3.Transform(Vector3.UnitX, Rotation);
    public Vector3 Up => Vector3.Transform(Vector3.UnitY, Rotation);

    public Vector3 Position
    {
        [MethodImpl(AggressiveInlining)]
        get => World.Get<C_Transform>(Entity).WorldPosition;
        [MethodImpl(AggressiveInlining)]
        set
        {
            // Convert World position to local space
            ref var hierarchy = ref World.Get<C_Hierarchy>(Entity);
            if (hierarchy.Parent == Entity.Null)
            {
                // Root actor: local = World
                LocalPosition = value;
            }
            else
            {
                // Has parent: calculate local from World
                ref var parentTransform = ref World.Get<C_Transform>(hierarchy.Parent);
                Vector3 localPos = value - parentTransform.WorldPosition;
                localPos = Vector3.Transform(localPos, Quaternion.Inverse(parentTransform.WorldRotation));
                localPos /= parentTransform.WorldScale;
                LocalPosition = localPos;
                // parentTransform.IsDirty = true;
            }
        }
    }

    public Quaternion Rotation
    {
        [MethodImpl(AggressiveInlining)]
        get => World.Get<C_Transform>(Entity).WorldRotation;
        [MethodImpl(AggressiveInlining)]
        set
        {
            ref var hierarchy = ref World.Get<C_Hierarchy>(Entity);
            if (hierarchy.Parent == Entity.Null)
            {
                LocalRotation = value;
            }
            else
            {
                ref var parentTransform = ref World.Get<C_Transform>(hierarchy.Parent);
                LocalRotation = Quaternion.Inverse(parentTransform.WorldRotation) * value;
            }
        }
    }

    public Vector3 Scale
    {
        [MethodImpl(AggressiveInlining)]
        get => World.Get<C_Transform>(Entity).WorldScale;
        [MethodImpl(AggressiveInlining)]
        set
        {
            ref var hierarchy = ref World.Get<C_Hierarchy>(Entity);
            if (hierarchy.Parent == Entity.Null)
            {
                LocalScale = value;
            }
            else
            {
                ref var parentTransform = ref World.Get<C_Transform>(hierarchy.Parent);
                LocalScale = value / parentTransform.WorldScale;
            }
        }
    }

    public Matrix4x4 WorldMatrix => World.Get<C_WorldTransformMatrix>(Entity).WorldMatrix;

    #endregion


}