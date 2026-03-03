namespace VoxelEngine.Core;

public record struct Int2
{

    public int X;
    public int Y;

    public Int2(int x, int y)
    {
        X = x;
        Y = y;
    }

    public static Int2 Zero => new(0, 0);
    public static Int2 One => new(1, 1);
    public static Int2 Two => new(2, 2);
    public static Int2 Up => new(0, 1);
    public static Int2 Down => new(0, -1);
    public static Int2 Left => new(-1, 0);
    public static Int2 Right => new(1, 0);

    public static Int2 operator +(Int2 a, Int2 b) => new(a.X + b.X, a.Y + b.Y);
    public static Int2 operator -(Int2 a, Int2 b) => new(a.X - b.X, a.Y - b.Y);
    public static Int2 operator *(Int2 a, int b) => new(a.X * b, a.Y * b);
    public static Int2 operator /(Int2 a, int b) => new(a.X / b, a.Y / b);

    public static Int2 operator *(Int2 a, Int2 b) => new(a.X * b.X, a.Y * b.Y);
    public static Int2 operator /(Int2 a, Int2 b) => new(a.X / b.X, a.Y / b.Y);

    public static Int2 operator *(int a, Int2 b) => new(a * b.X, a * b.Y);
    public static Int2 operator /(int a, Int2 b) => new(a / b.X, a / b.Y);
}