using Arch.Core;

namespace VoxelEngine.Core;

public abstract class Behavior : IBehavior
{
    protected Actor Self;
    public Actor Owner => Self;
    public Scene scene => Self.Scene;

    /// <summary>
    /// True after OnDestroy has been called. Accessing a destroyed behavior is invalid.
    /// Use this or the implicit bool operator to check: if (myBehavior) { ... }
    /// </summary>
    public bool IsDestroyed { get; private set; }

    internal void SetUp(Actor self)
    {
        Self = self;
        IsDestroyed = false;
    }

    /// <summary>
    /// Called internally by the registry after OnDestroy.
    /// Invalidates the behavior so stale references fail obviously.
    /// </summary>
    public void Invalidate()
    {
        IsDestroyed = true;
        Self = default; // zero out — accessing Self on a dead behavior gives a default (invalid) Actor
    }

    #region Helpers

    /// <summary>
    /// Queue actor for deferred destruction (processed between update phases).
    /// </summary>
    protected void Destroy(in Actor a)
    {
        // a.scene.CommandBuffer.DestroyDeferred(a.entity);
    }

    /// <summary>
    /// Queue entity for deferred destruction.
    /// </summary>
    protected void Destroy(in Entity e)
    {
        // Self.scene.CommandBuffer.DestroyDeferred(e);
    }

    // protected Actor Create()
    // {
    //     return default;
    //     // return Self.scene.CreateActor();
    // }

    // protected Entity CreateEntity()
    // {
    //     return default;
    //     // return Self.scene.CreateEntity();
    // }

    #endregion

    #region Lifecycle

    public virtual void OnAwake()
    {
    }

    public virtual void OnStart()
    {
    }

    // public virtual void OnCollision(in CollisionEvent ev)
    // {
    // }

    public virtual void OnDestroy()
    {
    }

    #endregion

    #region Operators

    /// <summary>
    /// Returns true if the behavior is alive (not destroyed).
    /// Usage: if (myBehavior) { myBehavior.DoSomething(); }
    /// </summary>
    public static implicit operator bool(Behavior? b) => b is not null && !b.IsDestroyed;

    public static implicit operator Actor(Behavior behavior) => behavior.Self;

    public override bool Equals(object? obj)
    {
        if (obj is Behavior other)
            return ReferenceEquals(this, other);
        return false;
    }

    public override int GetHashCode() => System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(this);

    public static bool operator ==(Behavior? a, Behavior? b)
    {
        bool aAlive = a is not null && !a.IsDestroyed;
        bool bAlive = b is not null && !b.IsDestroyed;

        if (!aAlive && !bAlive) return true; // both dead/null → equal
        if (!aAlive || !bAlive) return false; // one dead → not equal
        return ReferenceEquals(a, b);
    }

    public static bool operator !=(Behavior? a, Behavior? b) => !(a == b);

    #endregion
}


// /* 
// #region Event Subscription (Memory Leak Safe)

// public enum SceneEvent
// {
//     FixedUpdate,
//     Tick,
//     Render
// }

// private List<Action>? _cleanupActions;

// /// <summary>
// /// Safely subscribes to a specific scene event and ensures it is unsubscribed when this behavior is destroyed.
// /// </summary>
// protected void Subscribe(SceneEvent ev, Action myMethod)
// {
//     switch (ev)
//     {
//         case SceneEvent.FixedUpdate:
//             scene.OnFixedUpdate += myMethod;
//             _cleanupActions ??= new List<Action>();
//             _cleanupActions.Add(() => scene.OnFixedUpdate -= myMethod);
//             break;
//         case SceneEvent.Tick:
//             scene.OnTick += myMethod;
//             _cleanupActions ??= new List<Action>();
//             _cleanupActions.Add(() => scene.OnTick -= myMethod);
//             break;
//         case SceneEvent.Render:
//             scene.OnRender += myMethod;
//             _cleanupActions ??= new List<Action>();
//             _cleanupActions.Add(() => scene.OnRender -= myMethod);
//             break;
//     }
// }

// internal void PerformCleanup()
// {
//     if (_cleanupActions == null) return;
//     foreach (var action in _cleanupActions) action.Invoke();
//     _cleanupActions.Clear();
// }

// #endregion
//  */