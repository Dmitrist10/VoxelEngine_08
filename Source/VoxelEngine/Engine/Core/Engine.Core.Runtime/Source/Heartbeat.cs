using System.Diagnostics;
using VoxelEngine.Diagnostics;
using VoxelEngine.Graphics;

namespace VoxelEngine.Core.Runtime;

/// <summary>
/// Main game loop. Handles fixed timesteps, variable updates, and frame limiting.
/// </summary>
internal class Heartbeat
{
    // === Loop Control ===
    private bool _isRunning;
    private readonly Stopwatch _stopwatch;

    // === Timing Constants ===
    private const double FIXED_DELTA_TIME = Time.FixedDeltaTime; // 60 Hz physics
    private const double TICK_DELTA_TIME = Time.TickDeltaTime; // 20 Hz gameplay
    private const double MAX_FRAME_TIME = 0.25; // Prevents spiral of death (4 FPS minimum)

    // === Frame Time Smoothing ===
    // private const int FRAME_HISTORY_SIZE = 11;
    private const int FRAME_HISTORY_SIZE = 4;
    private readonly double[] _frameTimeHistory = new double[FRAME_HISTORY_SIZE];
    private int _frameHistoryIndex;
    private bool _frameHistoryFilled;

    // === FPS Tracking ===
    private int _frameCount;
    private double _fpsTimer;
    private const double FPS_UPDATE_INTERVAL = 1.0;

    // === Frame Rate Limiting ===
    private bool _hasFrameRateLimit;
    private double _targetFrameTime;

    public double TargetFrameRate { get; private set; }

    public Heartbeat()
    {
        _stopwatch = new Stopwatch();
        InitializeFrameHistory();
        SetTargetFrameRate(0);
    }

    /// <summary>
    /// Sets the target frame rate for the game loop. Use 0 or negative for unlimited FPS.
    /// </summary>
    public void SetTargetFrameRate(double targetFps)
    {
        TargetFrameRate = targetFps;
        _hasFrameRateLimit = targetFps > 0;
        _targetFrameTime = _hasFrameRateLimit ? 1.0 / targetFps : 0;

        Logger.Info(_hasFrameRateLimit
            ? $"Frame rate limit set to {targetFps:F1} FPS"
            : "Frame rate limit disabled (unlimited FPS)");
    }

    internal void Start(IUpdateCallbacksHandler callbacks, IWindowSurface window)
    {
        Logger.ExtraInfo("Starting Heartbeat");

        InitializeGameLoop(out double previousTime, out double accumulatorFixed, out double accumulatorTick);

        ModulesManager modulesManager = ServiceContainer.Get<ModulesManager>()!;
        modulesManager.Initialize();
        _stopwatch.Start();

        _isRunning = true;
        while (_isRunning)
        {
            double frameStartTime = _stopwatch.Elapsed.TotalSeconds;

            // Process window events (returns false if window closed)
            _isRunning = window.PumpEvents();
            if (!_isRunning) break;

            // Calculate frame timing
            double currentTime = _stopwatch.Elapsed.TotalSeconds;
            double rawFrameTime = currentTime - previousTime;
            previousTime = currentTime;

            // Clamp and smooth frame time
            double clampedFrameTime = Math.Min(rawFrameTime, MAX_FRAME_TIME);
            double smoothedFrameTime = SmoothFrameTime(clampedFrameTime);

            // Update global time system
            UpdateTimeSystem(smoothedFrameTime, rawFrameTime);

            // Track FPS metrics
            TrackFPS(rawFrameTime);

            // Run fixed-rate updates
            accumulatorFixed = RunFixedUpdates(callbacks, accumulatorFixed, clampedFrameTime);
            accumulatorTick = RunTickUpdates(callbacks, accumulatorTick, clampedFrameTime);

            // Update interpolation alpha for smooth rendering
            Time.UpdateAlpha(accumulatorFixed / FIXED_DELTA_TIME);

            // Run variable-rate updates
            modulesManager.Update();
            callbacks.Update();
            callbacks.Render();

            // Limit frame rate if configured
            LimitFrameRate(frameStartTime);
        }

        _stopwatch.Stop();
        Logger.ExtraInfo("Heartbeat Stopped");
    }

    internal void Stop() => _isRunning = false;

    // === Initialization ===

    private void InitializeFrameHistory()
    {
        // Pre-fill with 60 FPS estimate to prevent initial jitter
        for (int i = 0; i < FRAME_HISTORY_SIZE; i++)
            _frameTimeHistory[i] = 1.0 / 60.0;
    }

    private void InitializeGameLoop(out double previousTime, out double accumulatorFixed, out double accumulatorTick)
    {
        _stopwatch.Restart();
        previousTime = 0;
        accumulatorFixed = 0;
        accumulatorTick = 0;
        _frameCount = 0;
        _fpsTimer = 0;
        _frameHistoryIndex = 0;
        _frameHistoryFilled = false;
    }

    // === Fixed Update Loops ===

    private double RunFixedUpdates(IUpdateCallbacksHandler callbacks, double accumulator, double frameTime)
    {
        accumulator += frameTime;

        // Run fixed physics updates (60 Hz)
        while (accumulator >= FIXED_DELTA_TIME)
        {
            callbacks.FixedUpdate();
            accumulator -= FIXED_DELTA_TIME;
        }

        return accumulator;
    }

    private double RunTickUpdates(IUpdateCallbacksHandler callbacks, double accumulator, double frameTime)
    {
        accumulator += frameTime;

        // Run gameplay ticks (20 Hz)
        while (accumulator >= TICK_DELTA_TIME)
        {
            callbacks.Tick();
            accumulator -= TICK_DELTA_TIME;
        }

        return accumulator;
    }

    // === Time System ===

    private void UpdateTimeSystem(double smoothedFrameTime, double rawFrameTime)
    {
        Time.UpdateDelta(smoothedFrameTime);
        Time.UpdateRawDelta(rawFrameTime);
    }

    // === Frame Time Smoothing ===

    /// <summary>
    /// Uses median filter to smooth frame times and reduce jitter from outliers.
    /// </summary>
    private double SmoothFrameTime(double frameTime)
    {
        // Add to circular buffer
        _frameTimeHistory[_frameHistoryIndex] = frameTime;
        _frameHistoryIndex = (_frameHistoryIndex + 1) % FRAME_HISTORY_SIZE;

        if (_frameHistoryIndex == 0)
            _frameHistoryFilled = true;

        int count = _frameHistoryFilled ? FRAME_HISTORY_SIZE : _frameHistoryIndex;
        if (count == 0) return frameTime;

        // Sort and return median (robust against spikes)
        Span<double> sorted = stackalloc double[count];
        for (int i = 0; i < count; i++)
            sorted[i] = _frameTimeHistory[i];

        sorted.Sort();
        return sorted[count / 2];
    }

    // === FPS Tracking ===

    /// <summary>
    /// Tracks frame rate and updates Time system every second.
    /// </summary>
    private void TrackFPS(double frameTime)
    {
        _frameCount++;
        _fpsTimer += frameTime;

        if (_fpsTimer >= FPS_UPDATE_INTERVAL)
        {
            double fps = _frameCount / _fpsTimer;
            Time.UpdateFPS(fps, _frameCount);

            _frameCount = 0;
            _fpsTimer -= FPS_UPDATE_INTERVAL; // Preserve remainder for accuracy
        }
    }

    // === Frame Rate Limiting ===

    private void LimitFrameRate(double frameStartTime)
    {
        if (!_hasFrameRateLimit) return;

        double targetEndTime = frameStartTime + _targetFrameTime;
        double remaining = targetEndTime - _stopwatch.Elapsed.TotalSeconds;

        if (remaining <= 0) return; // Frame took longer than target, skip limiting

        // Use Thread.Sleep for bulk waiting (efficient but ~1-15ms precision)
        if (remaining > 0.002)
        {
            Thread.Sleep((int)((remaining - 0.001) * 1000));
        }

        // Spin-wait for remaining microseconds (high precision but CPU intensive)
        while (_stopwatch.Elapsed.TotalSeconds < targetEndTime)
        {
            Thread.SpinWait(10);
        }
    }
}
