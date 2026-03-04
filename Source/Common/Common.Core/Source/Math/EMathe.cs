using System.Numerics;

namespace VoxelEngine.Core;

/// <summary> Static math library providing various helper functions for games. </summary>
public static class EMath
{
    #region constants

    /// <summary> The mathematical constant PI. </summary>
    public const float PI = 3.14159265359f;
    /// <summary> Multiplier to convert degrees to radians. </summary>
    public const float Deg2Rad = PI / 180f;
    /// <summary> Multiplier to convert radians to degrees. </summary>
    public const float Rad2Deg = 180f / PI;
    
    public const float Epsilon = 1.401298E-45f;

    /// <summary> Lock object for thread-safe random operations. </summary>
    public static object RandomLock = new object();

    #endregion

    private static Random random = new Random();

    #region Basic Math

    /// <summary> Returns the cosine of f. </summary>
    public static float Cos(float f)
    {
        return (float)System.Math.Cos(f);
    }

    /// <summary> Returns the sine of f. </summary>
    public static float Sin(float f)
    {
        return (float)System.Math.Sin(f);
    }

    /// <summary> Returns the tangent of f. </summary>
    public static float Tan(float f)
    {
        return (float)System.Math.Tan(f);
    }

    /// <summary> Returns the square root of f. </summary>
    public static float Sqrt(float f)
    {
        return (float)System.Math.Sqrt(f);
    }

    /// <summary> Returns the absolute value of f. </summary>
    public static float Abs(float f)
    {
        return (float)System.Math.Abs(f);
    }

    /// <summary> Returns the absolute value of value. </summary>
    public static int Abs(int value)
    {
        return System.Math.Abs(value);
    }

    /// <summary> Returns the smallest of two values. </summary>
    public static float Min(float a, float b)
    {
        return a < b ? a : b;
    }

    /// <summary> Returns the largest of two values. </summary>
    public static float Max(float a, float b)
    {
        return a > b ? a : b;
    }

    /// <summary> Returns the smallest of two integers. </summary>
    public static int Min(int a, int b)
    {
        return a < b ? a : b;
    }

    /// <summary> Returns the largest of two integers. </summary>
    public static int Max(int a, int b)
    {
        return a > b ? a : b;
    }

    /// <summary> Returns f raised to power p. </summary>
    public static float Pow(float f, float p)
    {
        return (float)System.Math.Pow(f, p);
    }

    /// <summary> Returns the largest integer smaller to or equal to f. </summary>
    public static float Floor(float f)
    {
        return (float)System.Math.Floor(f);
    }

    /// <summary> Returns the smallest integer greater to or equal to f. </summary>
    public static float Ceil(float f)
    {
        return (float)System.Math.Ceiling(f);
    }

    /// <summary> Returns the largest integer smaller to or equal to f as an int. </summary>
    public static int FloorToInt(float f)
    {
        return (int)System.Math.Floor(f);
    }

    /// <summary> Returns the smallest integer greater to or equal to f as an int. </summary>
    public static int CeilToInt(float f)
    {
        return (int)System.Math.Ceiling(f);
    }

    /// <summary> Returns f rounded to the nearest integer. </summary>
    public static int RoundToInt(float f)
    {
        return (int)System.Math.Round(f);
    }

    #endregion

    #region Random

    /// <summary> Returns a random integer between min [inclusive] and max [inclusive]. </summary>
    public static int RandomRange(int min, int max)
    {
        return random.Next(min, max + 1);
    }
    /// <summary> Returns a thread-safe random integer between min [inclusive] and max [inclusive]. </summary>
    public static int RandomRangeThreadSafe(int min, int max)
    {
        lock (RandomLock)
        {
            return random.Next(min, max + 1);
        }
    }

    /// <summary> Returns a random float between min [inclusive] and max [inclusive]. </summary>
    public static float RandomRange(float min, float max)
    {
        return (float)(random.NextDouble() * (max - min) + min);
    }

    /// <summary> Returns a thread-safe random float between min [inclusive] and max [inclusive]. </summary>
    public static float RandomRangeThreadSafe(float min, float max)
    {
        lock (RandomLock)
        {
            return (float)(random.NextDouble() * (max - min) + min);
        }
    }

    /// <summary> Returns a random float value between 0.0 [inclusive] and 1.0 [inclusive]. </summary>
    public static float RandomValue()
    {
        return (float)random.NextDouble();
    }

    /// <summary> Returns a thread-safe random float value between 0.0 [inclusive] and 1.0 [inclusive]. </summary>
    public static float RandomValueThreadSafe()
    {
        lock (RandomLock)
        {
            return (float)random.NextDouble();
        }
    }

    #endregion

    #region Functions

    /// <summary> Clamps a value between min and max. </summary>
    public static float Clamp(float value, float min, float max)
    {
        if (value < min) return min;
        if (value > max) return max;
        return value;
    }

    /// <summary> Clamps a value between min and max. </summary>
    public static int Clamp(int value, int min, int max)
    {
        if (value < min) return min;
        if (value > max) return max;
        return value;
    }

    /// <summary> Clamps a value between 0 and 1. </summary>
    public static float Clamp01(float value)
    {
        if (value < 0f) return 0f;
        if (value > 1f) return 1f;
        return value;
    }

    /// <summary> Linearly interpolates between a and b by t. </summary>
    public static float Lerp(float a, float b, float t)
    {
        return a + (b - a) * Clamp01(t);
    }

    /// <summary> Linearly interpolates between a and b by t without clamping t. </summary>
    public static float LerpUnclamped(float a, float b, float t)
    {
        return a + (b - a) * t;
    }

    /// <summary> Linearly interpolates between two colors by t. </summary>
    public static Color Lerp(Color a, Color b, float t)
    {
        t = Clamp01(t);
        return new Color(
            a.R + (b.R - a.R) * t,
            a.G + (b.G - a.G) * t,
            a.B + (b.B - a.B) * t,
            a.A + (b.A - a.A) * t
        );
    }

    /// <summary> Linearly interpolates between two colors by t without clamping t. </summary>
    public static Color LerpUnclamped(Color a, Color b, float t)
    {
        return new Color(
            a.R + (b.R - a.R) * t,
            a.G + (b.G - a.G) * t,
            a.B + (b.B - a.B) * t,
            a.A + (b.A - a.A) * t
        );
    }

    /// <summary> 
    /// Calculates the linear parameter t that produces the interpolant value within the range [a, b].  
    /// Basically pass in <c>0</c>, <c>10</c> and <c>5</c> and it returns <c>0.5f</c> since <c>5</c> is halfway between <c>0</c> and <c>10</c>.
    ///  </summary>
    public static float InverseLerp(float a, float b, float value)
    {
        if (a != b)
            return Clamp01((value - a) / (b - a));
        return 0f;
    }

    #endregion

    #region SmoothStep

    /// <summary> Standard SmoothStep [3t² - 2t³]. Returns a value between 0 and 1. </summary>
    public static float SmoothStep(float t)
    {
        t = Clamp01(t);
        return t * t * (3f - 2f * t);
    }

    /// <summary> SmoothStep interpolation between a and b by t. </summary>
    public static float SmoothStep(float a, float b, float t)
    {
        return Lerp(a, b, SmoothStep(t));
    }

    /// <summary> SmoothStep interpolation between two Vector2 by t. </summary>
    public static Vector2 SmoothStep(Vector2 a, Vector2 b, float t)
    {
        return Vector2.Lerp(a, b, SmoothStep(t));
    }

    /// <summary> SmoothStep interpolation between two Vector3 by t. </summary>
    public static Vector3 SmoothStep(Vector3 a, Vector3 b, float t)
    {
        return Vector3.Lerp(a, b, SmoothStep(t));
    }

    /// <summary> SmoothStep interpolation between two Vector4 by t. </summary>
    public static Vector4 SmoothStep(Vector4 a, Vector4 b, float t)
    {
        return Vector4.Lerp(a, b, SmoothStep(t));
    }

    /// <summary> SmoothStep interpolation between two colors by t. </summary>
    public static Color SmoothStep(Color a, Color b, float t)
    {
        return Lerp(a, b, SmoothStep(t));
    }

    /// <summary> Improved SmootherStep [6t⁵ - 15t⁴ + 10t³]. Returns a value between 0 and 1. </summary>
    public static float SmootherStep(float t)
    {
        t = Clamp01(t);
        return t * t * t * (t * (t * 6f - 15f) + 10f);
    }

    /// <summary> SmootherStep interpolation between a and b by t. </summary>
    public static float SmootherStep(float a, float b, float t)
    {
        return Lerp(a, b, SmootherStep(t));
    }

    /// <summary> SmootherStep interpolation between two Vector2 by t. </summary>
    public static Vector2 SmootherStep(Vector2 a, Vector2 b, float t)
    {
        return Vector2.Lerp(a, b, SmootherStep(t));
    }

    /// <summary> SmootherStep interpolation between two Vector3 by t. </summary>
    public static Vector3 SmootherStep(Vector3 a, Vector3 b, float t)
    {
        return Vector3.Lerp(a, b, SmootherStep(t));
    }

    /// <summary> SmootherStep interpolation between two Vector4 by t. </summary>
    public static Vector4 SmootherStep(Vector4 a, Vector4 b, float t)
    {
        return Vector4.Lerp(a, b, SmootherStep(t));
    }

    /// <summary> SmootherStep interpolation between two colors by t. </summary>
    public static Color SmootherStep(Color a, Color b, float t)
    {
        return Lerp(a, b, SmootherStep(t));
    }

    #endregion

    #region MoveTowards

    /// <summary> Moves a value current towards target by maxDelta. </summary>
    public static float MoveTowards(float current, float target, float maxDelta)
    {
        if (Abs(target - current) <= maxDelta)
            return target;
        return current + Sign(target - current) * maxDelta;
    }

    /// <summary> Moves a Vector2 current towards target by maxDelta. </summary>
    public static Vector2 MoveTowards(Vector2 current, Vector2 target, float maxDelta)
    {
        Vector2 toVector = target - current;
        float dist = toVector.Length();
        if (dist <= maxDelta || dist == 0f)
            return target;
        return current + toVector / dist * maxDelta;
    }

    /// <summary> Moves a Vector3 current towards target by maxDelta. </summary>
    public static Vector3 MoveTowards(Vector3 current, Vector3 target, float maxDelta)
    {
        Vector3 toVector = target - current;
        float dist = toVector.Length();
        if (dist <= maxDelta || dist == 0f)
            return target;
        return current + toVector / dist * maxDelta;
    }

    #endregion

    #region LerpExp

    /// <summary> Exponential smoothing for a float value. Frame-rate independent. </summary>
    public static float LerpExp(float a, float b, float smoothTime, float deltaTime)
    {
        return Lerp(a, b, 1f - System.MathF.Exp(-10f / Max(smoothTime, 0.001f) * deltaTime));
    }

    /// <summary> Exponential smoothing for a Vector2. Frame-rate independent. </summary>
    public static Vector2 LerpExp(Vector2 a, Vector2 b, float smoothTime, float deltaTime)
    {
        return Vector2.Lerp(a, b, 1f - System.MathF.Exp(-10f / Max(smoothTime, 0.001f) * deltaTime));
    }

    /// <summary> Exponential smoothing for a Vector3. Frame-rate independent. </summary>
    public static Vector3 LerpExp(Vector3 a, Vector3 b, float smoothTime, float deltaTime)
    {
        return Vector3.Lerp(a, b, 1f - System.MathF.Exp(-10f / Max(smoothTime, 0.001f) * deltaTime));
    }

    /// <summary> Exponential smoothing for a Quaternion using Slerp. Frame-rate independent. </summary>
    public static Quaternion LerpExp(Quaternion a, Quaternion b, float smoothTime, float deltaTime)
    {
        return Quaternion.Slerp(a, b, 1f - System.MathF.Exp(-10f / Max(smoothTime, 0.001f) * deltaTime));
    }

    /// <summary> Exponential smoothing for a Vector4. Frame-rate independent. </summary>
    public static Vector4 LerpExp(Vector4 a, Vector4 b, float smoothTime, float deltaTime)
    {
        return Vector4.Lerp(a, b, 1f - System.MathF.Exp(-10f / Max(smoothTime, 0.001f) * deltaTime));
    }

    /// <summary> Exponential smoothing for a Color. Frame-rate independent. </summary>
    public static Color LerpExp(Color a, Color b, float smoothTime, float deltaTime)
    {
        return Lerp(a, b, 1f - System.MathF.Exp(-10f / Max(smoothTime, 0.001f) * deltaTime));
    }

    #endregion

    #region Other

    /// <summary> Returns the sign of f. Returns 1 if positive or zero, -1 if negative. </summary>
    public static float Sign(float f)
    {
        return f >= 0f ? 1f : -1f;
    }

    /// <summary> Wraps a value between min [inclusive] and max [inclusive]. </summary>
    public static int Wrap(int value, int min, int max)
    {
        int range = max - min + 1;
        return min + ((((value - min) % range) + range) % range);
    }

    /// <summary> Wraps a float value between min [inclusive] and max [inclusive]. </summary>
    public static float Wrap(float value, float min, float max)
    {
        float range = max - min + 1;
        return min + ((((value - min) % range) + range) % range);
    }

    /// <summary> Returns a random point in a sphere with radius 1. </summary>
    public static Vector3 RandomSphere()
    {
        return new Vector3(RandomValue(), RandomValue(), RandomValue());
    }

    /// <summary> Returns a random point in a sphere with a given radius. </summary>
    public static Vector3 RandomSphere(int radius)
    {
        return new Vector3(RandomValue(), RandomValue(), RandomValue()) * radius;
    }

    /// <summary> Returns a random point in a circle with radius 1. </summary>
    public static Vector2 RandomCircle()
    {
        return new Vector2(RandomValue(), RandomValue());
    }

    /// <summary> Returns a random point in a circle with a given radius. </summary>
    public static Vector2 RandomCircle(int radius)
    {
        return new Vector2(RandomValue(), RandomValue()) * radius;
    }

    #endregion
}