using System.Numerics;

namespace VoxelEngine.Input;

public interface IInput
{
    // ── Mouse ──────────────────────────────────────────────────────

    Vector2 MousePosition { get; }
    Vector2 MouseDelta { get; }
    Vector2 MouseScrollDelta { get; }

    /// <summary> Gets or sets whether the mouse cursor is locked and hidden. </summary>
    bool IsCursorLocked { get; set; }

    // ── Keyboard ───────────────────────────────────────────────────

    bool GetKey(Key key);
    bool GetKeyDown(Key key);
    bool GetKeyUp(Key key);

    // ── Mouse Buttons ──────────────────────────────────────────────

    bool GetMouseButton(MouseButton button);
    bool GetMouseButtonDown(MouseButton button);
    bool GetMouseButtonUp(MouseButton button);

    // ── Named Mappings (Axes & Actions) ────────────────────────────

    /// <summary>Returns -1, 0, or 1 based on negative/positive key state.</summary>
    float GetAxis(string name);

    bool GetAction(string name);
    bool GetActionDown(string name);

    /// <summary>True while any key bound to this action is held.</summary>
    bool GetButton(string name);

    /// <summary>True on the frame a bound key is first pressed.</summary>
    bool GetButtonDown(string name);

    /// <summary>True on the frame a bound key is released.</summary>
    bool GetButtonUp(string name);

    // ── Events ─────────────────────────────────────────────────────

    event Action<Key> OnKeyDown;
    event Action<Key> OnKeyUp;
    event Action<Key> OnKey;

    event Action<MouseButton> OnMouseButtonDown;
    event Action<MouseButton> OnMouseButtonUp;


    void RegisterAxisMapping(string name, Key positive, Key negative);
    void RegisterActionMapping(string name, Key key);
    void RegisterActionMapping(string name, MouseButton button);

}