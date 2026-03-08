using Silk.NET.Input;
using Silk.NET.Windowing;
using VoxelEngine.Graphics;

namespace VoxelEngine.Input;

public sealed class InputModule : IInputModule
{
    private NativeInputSource? _nativeSource;
    private InputContext? _context;

    public void OnInitialize()
    {

    }
    public void OnLoad(IWindowSurface surface)
    {
        _nativeSource = new NativeInputSource();
        IWindow window = (surface.NativeWindow as IWindow)!;
        IInputContext input = window.CreateInput();
        _nativeSource.Init(input);
        _context = new(_nativeSource);

        _nativeSource.OnKeyEvent += _context.ProcessKeyEvent;
        _nativeSource.OnMouseButtonEvent += _context.ProcessMouseButtonEvent;
        _nativeSource.OnMouseMoveEvent += _context.ProcessMouseMoveEvent;
        _nativeSource.OnMouseScrollEvent += _context.ProcessMouseScrollEvent;
    }


    public IInput GetInput()
    {
        return _context!;
    }

    public void OnRender()
    {
    }

    public void OnUpdate()
    {
        _context?.Update();
    }

    public void Cleanup()
    {
    }
}