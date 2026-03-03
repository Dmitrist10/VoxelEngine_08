namespace VoxelEngine.Core;

/// <summary>
/// Time Manager manages FPS, DeltaTime, and other time-related values.
/// </summary>
public static class Time
{
    /// <summary>
    /// Smoothed scaled delta time
    /// </summary>
    public static float DeltaTime { get; private set; }
    /// <summary>
    /// Smoothed unscaled delta time
    /// </summary>
    public static float UnscaledDeltaTime { get; private set; }

    /// <summary>
    /// Unsmoothed frame time
    /// </summary>
    public static float RawDeltaTime { get; private set; }

    public const float TickDeltaTime = 1f / 20f;
    public const float FixedDeltaTime = 1f / 60f;

    /// <summary>
    /// Total time since the game started (scaled by TimeScale)
    /// </summary>
    public static double TotalTime { get; private set; }
    /// <summary>
    /// Total time since the game started (unscaled)
    /// </summary>
    public static double UnscaledTotalTime { get; private set; }

    /// <summary>
    /// used to smooth physics for rendering
    /// </summary>
    public static float Alpha { get; private set; }

    // FPS tracking
    /// <summary>
    /// Current FPS
    /// </summary>
    public static double FPS { get; private set; }
    /// <summary>
    /// I actually don't know what this is for, but it's here.
    /// </summary>
    public static int FrameCount { get; private set; }

    /// <summary>
    /// Multiplier for DeltaTime and TotalTime
    /// </summary>
    public static float TimeScale { get; set; } = 1.0f;
    /// <summary>
    /// If 0, then FPS is not capped. If > 0, then FPS is capped to this value.
    /// </summary>
    public static int TargetFrameRate { get; set; } = 60;


    public static void UpdateFPS(double fps, int frameCount)
    {
        FPS = fps;
        FrameCount = frameCount;
    }
    public static void UpdateDelta(double deltaTime)
    {
        DeltaTime = (float)(deltaTime * TimeScale);
        UnscaledDeltaTime = (float)deltaTime;

        TotalTime += DeltaTime;
        UnscaledTotalTime += UnscaledDeltaTime;
    }
    public static void UpdateAlpha(double alpha)
    {
        Alpha = (float)alpha;
    }
    public static void UpdateRawDelta(double rawDeltaTime)
    {
        RawDeltaTime = (float)rawDeltaTime;
    }

    public static bool IsFrameRateTargeted => TargetFrameRate > 0;

    public static void SetTargetFrameRate(int fps)
    {
        TargetFrameRate = fps;
    }
}