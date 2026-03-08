using VoxelEngine.Core;

namespace VoxelEngine.Input;

// ────────────────────────────────────────────────────────────────────
//  Base
// ────────────────────────────────────────────────────────────────────

/// <summary>
/// Base class for all named input mappings (axes and actions).
/// </summary>
public abstract class InputMapping
{
    public string Name { get; }

    protected InputMapping(string name)
    {
        Name = name;
    }
}

// ────────────────────────────────────────────────────────────────────
//  Action  –  binary on/off (Jump, Sprint, Fire …)
// ────────────────────────────────────────────────────────────────────

/// <summary>
/// Maps a named action to one or more keys / mouse buttons.
/// Any bound input being active counts as "pressed".
/// </summary>
public sealed class ActionMapping : InputMapping
{
    public List<Key> Keys { get; } = new();
    public List<MouseButton> MouseButtons { get; } = new();

    public ActionMapping(string name, Key key) : base(name)
    {
        Keys.Add(key);
    }

    public ActionMapping(string name, MouseButton button) : base(name)
    {
        MouseButtons.Add(button);
    }
}

// ────────────────────────────────────────────────────────────────────
//  Axis  –  float -1 … +1 (Horizontal, Vertical …)
// ────────────────────────────────────────────────────────────────────

/// <summary>
/// Maps a named axis to positive / negative key pairs.
/// Multiple pairs can be registered (e.g. D/A and Right/Left → "Horizontal").
/// </summary>
public sealed class AxisMapping : InputMapping
{
    /// <summary>Each entry is (positive, negative).</summary>
    public List<(Key Positive, Key Negative)> KeyPairs { get; } = new();

    public AxisMapping(string name, Key positive, Key negative) : base(name)
    {
        KeyPairs.Add((positive, negative));
    }
}
