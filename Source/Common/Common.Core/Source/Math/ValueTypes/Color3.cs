using System.Numerics;

namespace VoxelEngine.Core;

public struct Color3
{
    public float R;
    public float G;
    public float B;

    public Color3(float r = 1, float g = 1, float b = 1)
    {
        R = r;
        G = g;
        B = b;
    }

    public static readonly Color3 Black = new Color3(0, 0, 0);
    public static readonly Color3 White = new Color3(1, 1, 1);
    public static readonly Color3 CornflowerBlue = new Color3(0.3921f, 0.5843f, 0.9294f);
    public static readonly Color3 Red = new Color3(1, 0, 0);
    public static readonly Color3 Green = new Color3(0, 1, 0);
    public static readonly Color3 Blue = new Color3(0, 0, 1);
    public static readonly Color3 Yellow = new Color3(1, 1, 0);
    public static readonly Color3 Cyan = new Color3(0, 1, 1);
    public static readonly Color3 Magenta = new Color3(1, 0, 1);
    public static readonly Color3 Orange = new Color3(1, 0.5f, 0);
    public static readonly Color3 Purple = new Color3(0.5f, 0, 0.5f);
    public static readonly Color3 Brown = new Color3(0.5f, 0.5f, 0);

    public static readonly Color3 RandomStatic =
        new Color3(EMathe.RandomValue(), EMathe.RandomValue(), EMathe.RandomValue());

    public static readonly Color3 RandomDarkStatic = new Color3(EMathe.RandomValue() * 0.75f,
        EMathe.RandomValue() * 0.75f, EMathe.RandomValue() * 0.75f);

    public static Color3 Random =>
        new Color3(EMathe.RandomValue(), EMathe.RandomValue(), EMathe.RandomValue());

    internal Vector3 ToVector3()
    {
        return new Vector3(R, G, B);
    }

    public static implicit operator Vector3(Color3 color)
    {
        return color.ToVector3();
    }

    public static implicit operator Color3(Vector3 vector)
    {
        return new Color3(vector.X, vector.Y, vector.Z);
    }

    public static implicit operator Color(Color3 color)
    {
        return new Color(color.R, color.G, color.B, 1);
    }

    public static implicit operator Color3(Color color)
    {
        return new Color3(color.R, color.G, color.B);
    }
}
