using System.Numerics;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace VoxelEngine.Graphics.OpenGL;

public sealed unsafe partial class GL_GraphicsDevice : IGraphicsDevice
{
    public IGraphicsFactory Factory { get; private set; } = null!;

    private IWindowSurface _window;
    private IWindow _nativeWindow = null!;

    private GL _GL = null!;
    private GL_AssetsManager _assetsManager = null!;

    private List<GL_GraphicsCommandsList> _commandLists = new();

    public GL_GraphicsDevice(IWindowSurface surface)
    {
        _window = surface;
        _window.OnLoad += OnWindowLoad;
        _window.OnResize += OnWindowResize;
    }

    private void OnWindowLoad()
    {
        _nativeWindow = (_window.NativeWindow as IWindow)!;
        _GL = GL.GetApi(_nativeWindow);

        _assetsManager = new GL_AssetsManager();

        Factory = new GL_GraphicsFactory(_GL, _assetsManager);
    }
    private void OnWindowResize(Vector2 vector)
    {
        _GL.Viewport(0, 0, (uint)vector.X, (uint)vector.Y);
    }


    public void Render()
    {
        foreach (var list in _commandLists)
            list.Execute();

        _commandLists.Clear();
    }
    public void Present()
    {
        _window.SwapBuffers();
    }


    public void Submit(IGraphicsCommandsList list)
    {
        _commandLists.Add((GL_GraphicsCommandsList)list);
    }

    public void Submit(ReadOnlySpan<IGraphicsCommandsList> lists)
    {
        foreach (var list in lists)
            _commandLists.Add((GL_GraphicsCommandsList)list);
    }

    public void Dispose()
    {
    }


}
