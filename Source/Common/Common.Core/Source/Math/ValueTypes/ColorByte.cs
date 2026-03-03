// using System.Numerics;

// namespace VoxelEngine.Core;

// public record struct ColorByte
// {
//     public byte R;
//     public byte G;
//     public byte B;
//     public byte A;

//     public ColorByte(byte r = 255, byte g = 255, byte b = 255, byte a = 255)
//     {
//         R = r;
//         G = g;
//         B = b;
//         A = a;
//     }

//     public static readonly ColorByte Black = new ColorByte(0, 0, 0, 255);
//     public static readonly ColorByte White = new ColorByte(255, 255, 255, 255);
//     public static readonly ColorByte CornflowerBlue = new ColorByte(100, 149, 237, 255);
//     public static readonly ColorByte Red = new ColorByte(255, 0, 0, 255);
//     public static readonly ColorByte Green = new ColorByte(0, 255, 0, 255);
//     public static readonly ColorByte Blue = new ColorByte(0, 0, 255, 255);
//     public static readonly ColorByte Yellow = new ColorByte(255, 255, 0, 255);
//     public static readonly ColorByte Cyan = new ColorByte(0, 255, 255, 255);
//     public static readonly ColorByte Magenta = new ColorByte(255, 0, 255, 255);
//     public static readonly ColorByte Orange = new ColorByte(255, 165, 0, 255);
//     public static readonly ColorByte Purple = new ColorByte(128, 0, 128, 255);
//     public static readonly ColorByte Brown = new ColorByte(165, 42, 42, 255);
//     public static readonly ColorByte Gray = new ColorByte(128, 128, 128, 255);

//     public static readonly ColorByte RandomStatic =
//         new ColorByte(EMathe.RandomValue(), EMathe.RandomValue(), EMathe.RandomValue(), 255);

//     public static readonly ColorByte RandomDarkStatic = new ColorByte(EMathe.RandomValue() * 0.75f,
//         EMathe.RandomValue() * 0.75f, EMathe.RandomValue() * 0.75f, 255);

//     public static ColorByte Random =>
//         new ColorByte(EMathe.RandomValue(), EMathe.RandomValue(), EMathe.RandomValue(), 255);

//     internal Vector4 ToVector4()
//     {
//         return new Vector4(R, G, B, A);
//     }

//     public static implicit operator Vector4(Color color)
//     {
//         return color.ToVector4();
//     }

//     public static implicit operator Color(Vector4 vector)
//     {
//         return new Color(vector.X, vector.Y, vector.Z, vector.W);
//     }

//     public static implicit operator Color3(Color color)
//     {
//         return new Color3(color.R, color.G, color.B);
//     }

//     public static implicit operator Color(Color3 color)
//     {
//         return new Color(color.R, color.G, color.B);
//     }

//     public static Color operator +(Color left, Color right)
//     {
//         return new Color(left.R + right.R, left.G + right.G, left.B + right.B, left.A + right.A);
//     }

//     public static Color operator -(Color left, Color right)
//     {
//         return new Color(left.R - right.R, left.G - right.G, left.B - right.B, left.A - right.A);
//     }

//     public static Color operator *(Color left, Color right)
//     {
//         return new Color(left.R * right.R, left.G * right.G, left.B * right.B, left.A * right.A);
//     }

//     public static Color operator /(Color left, Color right)
//     {
//         return new Color(left.R / right.R, left.G / right.G, left.B / right.B, left.A / right.A);
//     }

//     public static Color operator *(Color left, float right)
//     {
//         return new Color(left.R * right, left.G * right, left.B * right, left.A * right);
//     }

//     public static Color operator /(Color left, float right)
//     {
//         return new Color(left.R / right, left.G / right, left.B / right, left.A / right);
//     }

//     public static Color operator *(float left, Color right)
//     {
//         return new Color(left * right.R, left * right.G, left * right.B, left * right.A);
//     }

//     public static Color operator /(float left, Color right)
//     {
//         return new Color(left / right.R, left / right.G, left / right.B, left / right.A);
//     }

//     public static Color operator +(Color color, float value)
//     {
//         return new Color(color.R + value, color.G + value, color.B + value, color.A + value);
//     }

//     public static Color operator -(Color color, float value)
//     {
//         return new Color(color.R - value, color.G - value, color.B - value, color.A - value);
//     }
// }
