namespace VoxelEngine.Core;

// /// <summary>
// /// Base class for scene-level ECS systems that process entities via queries.
// /// </summary>
// public abstract class EntityProcessor
// {
//     public Scene scene { get; private set; } = null!;
//     // public World world => scene.World;

//     internal void SetUp(Scene s)
//     {
//         scene = s;
//     }

//     #region Helpers

//     /// <summary>
//     /// Queue actor for deferred destruction (processed between update phases).
//     /// </summary>
//     protected void Destroy(in Actor a)
//     {
//         // scene.CommandBuffer.DestroyDeferred(a.entity);
//     }

//     /// <summary>
//     /// Queue entity for deferred destruction.
//     /// </summary>
//     protected void Destroy(in Entity e)
//     {
//         // scene.CommandBuffer.DestroyDeferred(e);
//     }

//     #endregion

//     #region Lifecycle

//     public virtual void OnAwake()
//     {
//     }

//     public virtual void OnStart()
//     {
//     }

//     public virtual void OnUpdate()
//     {
//     }

//     public virtual void OnFixedUpdate()
//     {
//     }

//     public virtual void OnRender()
//     {
//     }

//     public virtual void OnTick()
//     {
//     }

//     public virtual void OnDestroy()
//     {
//     }

//     #endregion
// }

public interface IEntityProcessor
{
    void SetUp(Scene scene);

    void OnInitialize();
    void OnShutDown();
}
