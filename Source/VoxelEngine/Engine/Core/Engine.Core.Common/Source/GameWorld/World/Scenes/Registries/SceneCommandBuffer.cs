// namespace VoxelEngine.Core;

// /// <summary>
// /// Queues structural ECS changes to be flushed at safe sync points between update phases.
// /// Prevents iterator invalidation during update loops.
// /// 
// /// Usage:
// ///   commandBuffer.DestroyDeferred(entity);
// ///   commandBuffer.Enqueue(() => actor.AddComponent(new C_Health()));
// /// </summary>
// internal sealed class CommandBuffer
// {
//     private readonly Scene _scene;

//     // ── Destroy (hot path — dedicated queue for zero-alloc deferral) ──
//     private readonly HashSet<Entity> _destroySet = new();
//     private readonly List<Entity> _destroyQueue = new();

//     // ── Generic commands (add/remove component, spawn, etc.) ──
//     private readonly List<Action> _commands = new();

//     public CommandBuffer(Scene scene)
//     {
//         _scene = scene;
//     }

//     /// <summary>
//     /// Queue an entity for deferred destruction. Safe to call multiple times per entity.
//     /// </summary>
//     public void DestroyDeferred(in Entity entity)
//     {
//         if (_destroySet.Add(entity))
//             _destroyQueue.Add(entity);
//     }

//     /// <summary>
//     /// Queue any deferred operation (add/remove component, spawn, scene changes, etc.).
//     /// Executed in FIFO order during Flush.
//     /// </summary>
//     public void Enqueue(Action command)
//     {
//         _commands.Add(command);
//     }

//     /// <summary>
//     /// Process all queued commands. Called by Scene between update phases.
//     /// Order: generic commands first → destroy commands last (so deferred adds happen before cleanup).
//     /// </summary>
//     public void Flush()
//     {
//         // Generic commands first (spawns, component adds, etc.)
//         if (_commands.Count > 0)
//         {
//             for (int i = 0; i < _commands.Count; i++)
//                 _commands[i].Invoke();

//             _commands.Clear();
//         }

//         // Destroy commands last
//         if (_destroyQueue.Count > 0)
//         {
//             for (int i = 0; i < _destroyQueue.Count; i++)
//             {
//                 var entity = _destroyQueue[i];
//                 if (_scene.World.IsAlive(entity))
//                     _scene.Destroy(entity);
//             }

//             _destroyQueue.Clear();
//             _destroySet.Clear();
//         }
//     }

//     /// <summary>
//     /// Returns true if there are pending commands.
//     /// </summary>
//     public bool HasPending => _destroyQueue.Count > 0 || _commands.Count > 0;
// }


public sealed class SceneCommandBuffer : CommandBuffer
{

}