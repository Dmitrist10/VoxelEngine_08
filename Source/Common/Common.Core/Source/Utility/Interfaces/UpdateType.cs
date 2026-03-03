/* namespace VoxelEngine.Core;

public enum UpdateType
{
    Update = 0,
    FixedUpdate = 1,
    Render = 2,
    Tick = 3,
}

public interface IUpdateCallback
{
    void OnUpdate();
}

public interface IFixedUpdateCallback
{
    void OnFixedUpdate();
}

public interface ITickCallback
{
    void OnTick();
}

public interface IRenderCallback
{
    void OnRender();
}

public interface IShutDownCallback
{
    void OnShutDown();
}

public interface IStartCallback
{
    void OnStart();
}

public interface IAwakeCallback
{
    void OnAwake();
}

public interface IInitializeCallback
{
    void OnInit();
}

public interface IUpdatesCallbacks : IUpdateCallback, IFixedUpdateCallback, ITickCallback, IRenderCallback
{
}

public interface IStartCallbacks : IStartCallback, IAwakeCallback
{
}

public abstract class UpdateBase
{
    public virtual void OnUpdate()
    {
    }

    public virtual void OnFixedUpdate()
    {
    }

    public virtual void OnTick()
    {
    }

    public virtual void OnRender()
    {
    }

    public virtual void OnCleanUp()
    {
    }
}

// public virtual void OnStart()
//     {
//     }
//     public virtual void OnAwake()
//     {
//     }

// internal void OnUpdate()
// {
// }
// internal void OnFixedUpdate()
// {
// }
// internal void OnTick()
// {
// }
// internal void OnRender()
// {
// }
// internal void OnCleanUp()
// {
// } */