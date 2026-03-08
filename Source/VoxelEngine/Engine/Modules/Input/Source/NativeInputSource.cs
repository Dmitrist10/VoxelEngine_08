using System.Numerics;
using Silk.NET.Input;

namespace VoxelEngine.Input;

internal sealed class NativeInputSource
{
    public event Action<Key, bool>? OnKeyEvent;
    public event Action<MouseButton, bool>? OnMouseButtonEvent;
    public event Action<Vector2>? OnMouseMoveEvent;
    public event Action<Vector2>? OnMouseScrollEvent;

    private IInputContext? _context;
    public IReadOnlyList<IMouse> Mice => _context?.Mice ?? Array.Empty<IMouse>();

    public void Init(IInputContext input)
    {
        _context = input;
        foreach (var keyboard in input.Keyboards)
        {
            keyboard.KeyDown += (_, key, _) => OnKeyEvent?.Invoke(MapKey(key), true);
            keyboard.KeyUp += (_, key, _) => OnKeyEvent?.Invoke(MapKey(key), false);
        }

        foreach (var mouse in input.Mice)
        {
            mouse.MouseDown += (_, btn) => OnMouseButtonEvent?.Invoke(MapMouseButton(btn), true);
            mouse.MouseUp += (_, btn) => OnMouseButtonEvent?.Invoke(MapMouseButton(btn), false);
            mouse.MouseMove += (_, pos) => OnMouseMoveEvent?.Invoke(new Vector2(pos.X, pos.Y));
            mouse.Scroll += (_, scroll) => OnMouseScrollEvent?.Invoke(new Vector2(scroll.X, scroll.Y));
        }
    }

    private static Key MapKey(Silk.NET.Input.Key key)
    {
        // Direct cast — our Key enum mirrors GLFW keycodes used by Silk
        return (Key)(int)key;
    }

    private static MouseButton MapMouseButton(Silk.NET.Input.MouseButton btn)
    {
        return (MouseButton)(int)btn;
    }

}