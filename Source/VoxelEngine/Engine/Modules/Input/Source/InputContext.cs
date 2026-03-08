using System.Numerics;
using VoxelEngine.Diagnostics;

namespace VoxelEngine.Input;

public sealed class InputContext : IInput
{
    private readonly Queue<(Key key, bool down)> _pendingKeyEvents = new();
    private readonly Queue<(MouseButton btn, bool down)> _pendingMouseButtonEvents = new();

    // ── Raw key / button state ──────────────────────────────────────

    private readonly HashSet<Key> _currentKeys = new();
    private readonly HashSet<Key> _previousKeys = new();

    private readonly HashSet<MouseButton> _currentMouseButtons = new();
    private readonly HashSet<MouseButton> _previousMouseButtons = new();

    // ── Mouse position & delta ──────────────────────────────────────

    private Vector2 _currentMousePosition;
    private Vector2 _previousMousePosition;
    private bool _hasReceivedFirstPosition;

    private Vector2 _mousePosition;
    private Vector2 _mouseDelta;
    private Vector2 _mouseScrollDelta;
    private Vector2 _scrollAccumulator; // accumulates between Update() calls

    public Vector2 MousePosition => _mousePosition;
    public Vector2 MouseDelta => _mouseDelta;
    public Vector2 MouseScrollDelta => _mouseScrollDelta;

    private bool _isCursorLocked = true;
    public bool IsCursorLocked
    {
        get => _isCursorLocked;
        set
        {
            _isCursorLocked = value;
            foreach (var mouse in _source.Mice)
            {
                mouse.Cursor.CursorMode = value ? Silk.NET.Input.CursorMode.Raw : Silk.NET.Input.CursorMode.Normal;
            }
        }
    }

    // ── Named mappings ──────────────────────────────────────────────

    private readonly Dictionary<string, AxisMapping> _axisMappings = new();
    private readonly Dictionary<string, ActionMapping> _actionMappings = new();

    // ── Events (fired during Update flush) ──────────────────────────

    public event Action<Key>? OnKeyDown;
    public event Action<Key>? OnKeyUp;
    public event Action<Key>? OnKey;

    public event Action<MouseButton>? OnMouseButtonDown;
    public event Action<MouseButton>? OnMouseButtonUp;

    private readonly NativeInputSource _source;

    internal InputContext(NativeInputSource source)
    {
        _source = source;
        RegisterDefaults();
    }

    public void Update()
    {
        // 1. Snapshot previous frame state
        _previousKeys.Clear();
        _previousKeys.UnionWith(_currentKeys);

        _previousMouseButtons.Clear();
        _previousMouseButtons.UnionWith(_currentMouseButtons);

        // 2. Flush queued key events into current state
        while (_pendingKeyEvents.TryDequeue(out var evt))
        {
            if (evt.down)
                _currentKeys.Add(evt.key);
            else
                _currentKeys.Remove(evt.key);
        }

        // 3. Flush queued mouse button events into current state
        while (_pendingMouseButtonEvents.TryDequeue(out var evt))
        {
            if (evt.down)
                _currentMouseButtons.Add(evt.btn);
            else
                _currentMouseButtons.Remove(evt.btn);
        }

        // 4. Mouse position & delta
        _mouseDelta = _currentMousePosition - _previousMousePosition;
        _previousMousePosition = _currentMousePosition;
        _mousePosition = _currentMousePosition;

        // 5. Scroll: flush accumulated delta, then reset
        _mouseScrollDelta = _scrollAccumulator;
        _scrollAccumulator = Vector2.Zero;
    }

    private void RegisterDefaults()
    {
        RegisterAxisMapping("Horizontal", Key.A, Key.D);
        RegisterAxisMapping("Vertical", Key.W, Key.S);
        RegisterAxisMapping("UP_DOWN", Key.Space, Key.LeftControl);

        RegisterActionMapping("Jump", Key.Space);
        RegisterActionMapping("Sprint", Key.LeftShift);
    }

    // ═══════════════════════════════════════════════════════════════
    //  Mapping Registration
    // ═══════════════════════════════════════════════════════════════

    public void RegisterAxisMapping(string name, Key positive, Key negative)
    {
        if (_axisMappings.TryGetValue(name, out var existing))
        {
            existing.KeyPairs.Add((positive, negative));
        }
        else
        {
            _axisMappings[name] = new AxisMapping(name, positive, negative);
        }
    }

    public void RegisterActionMapping(string name, Key key)
    {
        if (_actionMappings.TryGetValue(name, out var existing))
        {
            existing.Keys.Add(key);
        }
        else
        {
            _actionMappings[name] = new ActionMapping(name, key);
        }
    }

    public void RegisterActionMapping(string name, MouseButton button)
    {
        if (_actionMappings.TryGetValue(name, out var existing))
        {
            existing.MouseButtons.Add(button);
        }
        else
        {
            _actionMappings[name] = new ActionMapping(name, button);
        }
    }

    public void RemoveMapping(string name)
    {
        _axisMappings.Remove(name);
        _actionMappings.Remove(name);
    }

    // ═══════════════════════════════════════════════════════════════
    //  State Queries — Raw Keys
    // ═══════════════════════════════════════════════════════════════

    public bool GetKey(Key key) => _currentKeys.Contains(key);
    public bool GetKeyDown(Key key) => _currentKeys.Contains(key) && !_previousKeys.Contains(key);
    public bool GetKeyUp(Key key) => !_currentKeys.Contains(key) && _previousKeys.Contains(key);
    // ═══════════════════════════════════════════════════════════════
    //  State Queries — Raw Mouse Buttons
    // ═══════════════════════════════════════════════════════════════

    public bool GetMouseButton(MouseButton button) => _currentMouseButtons.Contains(button);

    public bool GetMouseButtonDown(MouseButton button) =>
        _currentMouseButtons.Contains(button) && !_previousMouseButtons.Contains(button);

    public bool GetMouseButtonUp(MouseButton button) =>
        !_currentMouseButtons.Contains(button) && _previousMouseButtons.Contains(button);

    // ═══════════════════════════════════════════════════════════════
    //  State Queries — Named Mappings
    // ═══════════════════════════════════════════════════════════════

    public float GetAxis(string name)
    {
        if (!_axisMappings.TryGetValue(name, out var mapping))
            return 0f;

        float value = 0f;
        foreach (var (positive, negative) in mapping.KeyPairs)
        {
            if (GetKey(positive)) value += 1f;
            if (GetKey(negative)) value -= 1f;
        }

        return System.Math.Clamp(value, -1f, 1f);
    }

    public bool GetAction(string name)
    {
        if (!_actionMappings.TryGetValue(name, out var mapping))
            return false;

        foreach (var key in mapping.Keys)
            if (GetKey(key))
                return true;

        return false;
    }

    public bool GetActionDown(string name)
    {
        if (!_actionMappings.TryGetValue(name, out var mapping))
            return false;

        foreach (var key in mapping.Keys)
            if (GetKeyDown(key))
                return true;

        return false;
    }

    public bool GetButton(string name)
    {
        if (!_actionMappings.TryGetValue(name, out var mapping))
            return false;

        foreach (var key in mapping.Keys)
            if (GetKey(key))
                return true;

        foreach (var btn in mapping.MouseButtons)
            if (GetMouseButton(btn))
                return true;

        return false;
    }

    public bool GetButtonDown(string name)
    {
        if (!_actionMappings.TryGetValue(name, out var mapping))
            return false;

        foreach (var key in mapping.Keys)
            if (GetKeyDown(key))
                return true;

        foreach (var btn in mapping.MouseButtons)
            if (GetMouseButtonDown(btn))
                return true;

        return false;
    }

    public bool GetButtonUp(string name)
    {
        if (!_actionMappings.TryGetValue(name, out var mapping))
            return false;

        foreach (var key in mapping.Keys)
            if (GetKeyUp(key))
                return true;

        foreach (var btn in mapping.MouseButtons)
            if (GetMouseButtonUp(btn))
                return true;

        return false;
    }

    // ═══════════════════════════════════════════════════════════════
    //  Event Injection  (called by NativeInputSource — queued, not applied immediately)
    // ═══════════════════════════════════════════════════════════════

    internal void ProcessKeyEvent(Key key, bool isDown)
    {
        _pendingKeyEvents.Enqueue((key, isDown));

        // Fire events immediately (for listeners that want real-time notification)
        if (isDown)
        {
            OnKeyDown?.Invoke(key);
            OnKey?.Invoke(key);
        }
        else
        {
            OnKeyUp?.Invoke(key);
        }
    }

    internal void ProcessMouseButtonEvent(MouseButton button, bool isDown)
    {
        _pendingMouseButtonEvents.Enqueue((button, isDown));

        if (isDown)
            OnMouseButtonDown?.Invoke(button);
        else
            OnMouseButtonUp?.Invoke(button);
    }

    internal void ProcessMouseMoveEvent(Vector2 position)
    {
        _currentMousePosition = position;

        // On first event, snap previous so delta starts at zero
        if (!_hasReceivedFirstPosition)
        {
            _previousMousePosition = position;
            _hasReceivedFirstPosition = true;
        }
    }

    internal void ProcessMouseScrollEvent(Vector2 delta)
    {
        // Accumulate — multiple scroll events can arrive between Update() calls
        _scrollAccumulator += delta;
    }
}