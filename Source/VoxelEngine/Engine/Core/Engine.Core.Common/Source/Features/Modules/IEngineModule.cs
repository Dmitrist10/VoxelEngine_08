using VoxelEngine.Graphics;

namespace VoxelEngine.Core;

public interface IEngineModule
{
    void OnInitialize();
    void OnLoad(IWindowSurface window);
    void OnUpdate();
    // void OnRender();
    void Cleanup();
}
