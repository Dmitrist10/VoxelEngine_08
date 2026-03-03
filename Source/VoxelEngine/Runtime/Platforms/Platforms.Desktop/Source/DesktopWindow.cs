using System.Numerics;

using Silk.NET.Core;
using Silk.NET.Maths;
using Silk.NET.Windowing;

using VoxelEngine.Diagnostics;
using VoxelEngine.Graphics;

namespace VoxelEngine.Runtime.Platforms.Desktop;

internal sealed class DesktopWindow : IWindowSurface
{
    public Vector2 Size => new Vector2(window.Size.X, window.Size.Y);
    public object NativeWindow => window;

    public event Action<Vector2>? OnResize;
    public event Action? OnLoad;
    public event Action? OnLateLoad;

    private bool isRunning = true;
    private IWindow window;

    public DesktopWindow(string title, int width, int height)
    {
        Logger.Info($"Creating window: '{title}' ({width}x{height})");

        WindowOptions options = WindowOptions.Default;
        options.Title = title;
        options.Size = new Vector2D<int>(width, height);
        options.VSync = false;
        options.API = new GraphicsAPI(ContextAPI.OpenGL, ContextProfile.Core, ContextFlags.Default, new APIVersion(4, 6));

        window = Window.Create(options);

        window.Load += Load;
        window.Closing += Closing;
        window.FramebufferResize += Resize;

        Logger.Info("Desktop window created successfully.");
    }

    #region Window Events

    private void Load()
    {
        OnLoad?.Invoke();
        OnLateLoad?.Invoke();
    }

    private void Closing()
    {
        Logger.Info("Desktop window closing...");
        isRunning = false;
    }

    private void Resize(Vector2D<int> d)
    {
        OnResize?.Invoke(new Vector2(d.X, d.Y));
    }

    #endregion

    public void Initialize()
    {
        window.Initialize();
    }

    public bool PumpEvents()
    {
        window.DoEvents();
        return isRunning;
    }

    public void SwapBuffers()
    {
        window.SwapBuffers();
    }

    public void SetIcon(byte[] pixels, uint width, uint height)
    {
        Logger.Info("Setting window icon...");
        var icon = new RawImage((int)width, (int)height, pixels);

        window.SetWindowIcon([icon]);
    }
}