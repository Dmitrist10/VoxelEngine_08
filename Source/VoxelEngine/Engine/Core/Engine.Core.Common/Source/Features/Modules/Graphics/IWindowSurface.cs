using System.Numerics;

namespace VoxelEngine.Graphics;

public interface IWindowSurface
{
    Vector2 Size { get; }
    object NativeWindow { get; } // should return IWindow from silk.net

    event Action<Vector2> OnResize;
    event Action OnLoad;
    event Action OnLateLoad;

    void Initialize();

    void SetIcon(byte[] pixels, uint width, uint height);

    bool PumpEvents();
    void SwapBuffers();
}