using System.Numerics;

namespace VoxelEngine.Core;

public record struct Int3
{

    public int X;
    public int Y;
    public int Z;

    public Int3(int x, int y, int z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public static Int3 Zero => new(0, 0, 0);
    public static Int3 One => new(1, 1, 1);
    public static Int3 Two => new(2, 2, 2);

    public static Int3 Up => new(0, 1, 0);
    public static Int3 Down => new(0, -1, 0);

    public static Int3 Left => new(-1, 0, 0);
    public static Int3 Right => new(1, 0, 0);

    public static Int3 Forward => new(0, 0, 1);
    public static Int3 Back => new(0, 0, -1);

    public static Int3 UnitX => new(1, 0, 0);
    public static Int3 UnitY => new(0, 1, 0);
    public static Int3 UnitZ => new(0, 0, 1);

    public static Int3 MaxValue => new(int.MaxValue, int.MaxValue, int.MaxValue);
    // public static Int3 MinValue => new (int.MaxValue, int.MaxValue, int.MaxValue);

    public static Int3 operator +(Int3 a, Int3 b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    public static Int3 operator -(Int3 a, Int3 b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
    public static Int3 operator *(Int3 a, int b) => new(a.X * b, a.Y * b, a.Z * b);
    public static Int3 operator /(Int3 a, int b) => new(a.X / b, a.Y / b, a.Z / b);

    public static Int3 operator *(Int3 a, Int3 b) => new(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
    public static Int3 operator /(Int3 a, Int3 b) => new(a.X / b.X, a.Y / b.Y, a.Z / b.Z);

    public static Int3 operator *(int a, Int3 b) => new(a * b.X, a * b.Y, a * b.Z);
    public static Int3 operator /(int a, Int3 b) => new(a / b.X, a / b.Y, a / b.Z);

    public static int DistanceSquared(Int3 a, Int3 b)
    {
        int dx = a.X - b.X;
        int dy = a.Y - b.Y;
        int dz = a.Z - b.Z;
        return dx * dx + dy * dy + dz * dz;
    }
    // public static int Distance(Int3 a, Int3 b)
    // {
    //     int dx = a.X - b.X;
    //     int dy = a.Y - b.Y;
    //     int dz = a.Z - b.Z;
    //     return dx + dy + dz;
    // }

    public static explicit operator Vector3(Int3 v) => new(v.X, v.Y, v.Z);
}
