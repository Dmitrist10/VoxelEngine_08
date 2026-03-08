using System.Numerics;

namespace VoxelEngine.Input;

public static class InputManager
{

    public static IInput _input = null!;
    public static void Init(IInput input) => _input = input;

    // ── Keyboard ───────────────────────────────────────────────────

    public static bool GetKey(Key key) => _input.GetKey(key);
    public static bool GetKeyDown(Key key) => _input.GetKeyDown(key);
    public static bool GetKeyUp(Key key) => _input.GetKeyUp(key);

    // ── Mouse ──────────────────────────────────────────────────────

    public static Vector2 MousePosition => _input.MousePosition;
    public static Vector2 MouseDelta => _input.MouseDelta;
    public static Vector2 MouseScrollDelta => _input.MouseScrollDelta;

    public static bool IsCursorLocked
    {
        get => _input.IsCursorLocked;
        set => _input.IsCursorLocked = value;
    }

    // ── Mouse Buttons ──────────────────────────────────────────────

    public static bool GetMouseButton(MouseButton button) => _input.GetMouseButton(button);
    public static bool GetMouseButtonDown(MouseButton button) => _input.GetMouseButtonDown(button);
    public static bool GetMouseButtonUp(MouseButton button) => _input.GetMouseButtonUp(button);

    // ── Named Mappings ─────────────────────────────────────────────

    public static float GetAxis(string name) => _input.GetAxis(name);
    public static bool GetAction(string name) => _input.GetAction(name);
    public static bool GetActionDown(string name) => _input.GetActionDown(name);
    public static bool GetButton(string name) => _input.GetButton(name);
    public static bool GetButtonDown(string name) => _input.GetButtonDown(name);
    public static bool GetButtonUp(string name) => _input.GetButtonUp(name);

    public static void RegisterAxisMapping(string name, Key positive, Key negative) => _input.RegisterAxisMapping(name, positive, negative);
    public static void RegisterActionMapping(string name, Key key) => _input.RegisterActionMapping(name, key);
    public static void RegisterActionMapping(string name, MouseButton button) => _input.RegisterActionMapping(name, button);

}