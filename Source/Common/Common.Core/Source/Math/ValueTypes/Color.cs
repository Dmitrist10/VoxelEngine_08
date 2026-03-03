using System.Numerics;

namespace VoxelEngine.Core;

public record struct Color
{
    public float R;
    public float G;
    public float B;
    public float A;

    public Color(float r = 1, float g = 1, float b = 1, float a = 1)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }

    public static readonly Color Black = new Color(0, 0, 0, 1);
    public static readonly Color White = new Color(1, 1, 1, 1);
    public static readonly Color CornflowerBlue = new Color(0.3921f, 0.5843f, 0.9294f, 1);
    public static readonly Color Red = new Color(1, 0, 0, 1);
    public static readonly Color Green = new Color(0, 1, 0, 1);
    public static readonly Color Blue = new Color(0, 0, 1, 1);
    public static readonly Color Yellow = new Color(1, 1, 0, 1);
    public static readonly Color Cyan = new Color(0, 1, 1, 1);
    public static readonly Color Magenta = new Color(1, 0, 1, 1);
    public static readonly Color Orange = new Color(1, 0.5f, 0, 1);
    public static readonly Color Purple = new Color(0.5f, 0, 0.5f, 1);
    public static readonly Color Brown = new Color(0.5f, 0.5f, 0, 1);
    public static readonly Color Gray = new Color(0.25f, 0.25f, 0.25f, 1);

    public static readonly Color RandomStatic =
        new Color(EMathe.RandomValue(), EMathe.RandomValue(), EMathe.RandomValue(), 1);

    public static readonly Color RandomDarkStatic = new Color(EMathe.RandomValue() * 0.75f,
        EMathe.RandomValue() * 0.75f, EMathe.RandomValue() * 0.75f, 1);

    public static Color Random =>
        new Color(EMathe.RandomValue(), EMathe.RandomValue(), EMathe.RandomValue(), 1);

    internal Vector4 ToVector4()
    {
        return new Vector4(R, G, B, A);
    }

    public static implicit operator Vector4(Color color)
    {
        return color.ToVector4();
    }

    public static implicit operator Color(Vector4 vector)
    {
        return new Color(vector.X, vector.Y, vector.Z, vector.W);
    }

    public static implicit operator Color3(Color color)
    {
        return new Color3(color.R, color.G, color.B);
    }

    public static implicit operator Color(Color3 color)
    {
        return new Color(color.R, color.G, color.B);
    }

    public static Color operator +(Color left, Color right)
    {
        return new Color(left.R + right.R, left.G + right.G, left.B + right.B, left.A + right.A);
    }

    public static Color operator -(Color left, Color right)
    {
        return new Color(left.R - right.R, left.G - right.G, left.B - right.B, left.A - right.A);
    }

    public static Color operator *(Color left, Color right)
    {
        return new Color(left.R * right.R, left.G * right.G, left.B * right.B, left.A * right.A);
    }

    public static Color operator /(Color left, Color right)
    {
        return new Color(left.R / right.R, left.G / right.G, left.B / right.B, left.A / right.A);
    }

    public static Color operator *(Color left, float right)
    {
        return new Color(left.R * right, left.G * right, left.B * right, left.A * right);
    }

    public static Color operator /(Color left, float right)
    {
        return new Color(left.R / right, left.G / right, left.B / right, left.A / right);
    }

    public static Color operator *(float left, Color right)
    {
        return new Color(left * right.R, left * right.G, left * right.B, left * right.A);
    }

    public static Color operator /(float left, Color right)
    {
        return new Color(left / right.R, left / right.G, left / right.B, left / right.A);
    }

    public static Color operator +(Color color, float value)
    {
        return new Color(color.R + value, color.G + value, color.B + value, color.A + value);
    }

    public static Color operator -(Color color, float value)
    {
        return new Color(color.R - value, color.G - value, color.B - value, color.A - value);
    }
}
