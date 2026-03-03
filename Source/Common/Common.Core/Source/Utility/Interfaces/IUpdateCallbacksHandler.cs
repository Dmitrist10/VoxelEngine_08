namespace VoxelEngine.Core;

public interface IUpdateCallbacksHandler
{
    void Init();

    void Update();
    void FixedUpdate();
    void Tick();
    void Render();

    void Cleanup();
}

public interface IUpdatable { void OnUpdate(); }
public interface IFixedUpdatable { void OnFixedUpdate(); }
public interface IRenderable { void OnRender(); }
public interface ITickable { void OnTick(); }

// public interface IUpdate
// {
//     void OnUpdate();
// }

// public interface IFixedUpdate
// {
//     void OnFixedUpdate();
// }

// public interface ITick
// {
//     void OnTick();
// }

// public interface IRender
// {
//     void OnRender();
// }
