using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace VoxelEngine.Diagnostics;

public class Logger
{
    private static Logger? _instance;
    private static readonly object _lock = new object();

    private readonly StreamWriter? _fileWriter;
    private readonly string? _logFilePath;
    private readonly LogLevel _minimumLevel;

    // Configuration
    private readonly bool _writeToConsole;
    private readonly bool _writeToFile;
    private readonly bool _useColors;
    private readonly bool _includeCallerInfo;
    private readonly HashSet<LogCategory>? _enabledCategories;

    private readonly int _levelPadding;

    public static Logger Instance
    {
        get
        {
            if (_instance == null)
            {
                throw new InvalidOperationException("Logger not initialized. Call Logger.Initialize() first.");
            }

            return _instance;
        }
    }

    private Logger(LoggerConfig config)
    {
        _minimumLevel = config.MinimumLevel;
        _writeToConsole = config.WriteToConsole;
        _writeToFile = config.WriteToFile;
        _useColors = config.UseColors;
        _includeCallerInfo = config.IncludeCallerInfo;
        _levelPadding = config.LevelPadding;
        _enabledCategories = config.EnabledCategories != null
            ? new HashSet<LogCategory>(config.EnabledCategories)
            : null;

#if LOGGING
        if (_writeToFile)
        {
            // Create logs directory if it doesn't exist
            string logsDir = config.LogDirectory ?? "logs";
            if (!Directory.Exists(logsDir))
            {
                Directory.CreateDirectory(logsDir);
            }

            // Generate timestamped filename: engine_20260116_193552.log
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string filename = config.FileNamePattern?.Replace("{timestamp}", timestamp)
                              ?? $"engine_{timestamp}.log";

            _logFilePath = Path.Combine(logsDir, filename);

            try
            {
                _fileWriter = new StreamWriter(_logFilePath, append: true)
                {
                    AutoFlush = true // Ensure logs are written immediately
                };

                // Write header
                _fileWriter.WriteLine("=".PadRight(80, '='));
                _fileWriter.WriteLine($"Engine Log - Started at {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                _fileWriter.WriteLine("=".PadRight(80, '='));
                _fileWriter.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Logger] Failed to create log file: {ex.Message}");
                _fileWriter = null;
            }
        }
#endif
    }

    public static void Initialize(LoggerConfig? config = null, string message = "starting from main")
    {
        lock (_lock)
        {
            if (_instance != null)
            {
                return; // Already initialized
            }

            config ??= new LoggerConfig(); // Use defaults if none provided
            _instance = new Logger(config);

            // Only log initialization message in Debug builds
#if DEBUG
            _instance.Log(LogLevel.Info, message);
#endif
        }
    }

    public static void Shutdown()
    {
#if LOGGING
        lock (_lock)
        {
            if (_instance != null)
            {
                _instance._fileWriter?.WriteLine();
                _instance._fileWriter?.WriteLine("=".PadRight(80, '='));
                _instance._fileWriter?.WriteLine($"Engine Log - Ended at {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                _instance._fileWriter?.WriteLine("=".PadRight(80, '='));
                _instance._fileWriter?.Flush();
                _instance._fileWriter?.Close();
                _instance._fileWriter?.Dispose();
                _instance = null;
            }
        }
#endif
    }

    // ========================================
    // Public static logging methods
    // Can be toggled via LOGGING constant (see Directory.Build.props)
    // using [Conditional("LOGGING")] attribute
    // ========================================

    [Conditional("LOGGING")]
    public static void Debug(
        string message,
        LogCategory category = LogCategory.Core,
        [CallerMemberName] string caller = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
        => Instance.LogWithCategory(LogLevel.Debug, category, message, caller, filePath, lineNumber);

    [Conditional("LOGGING")]
    public static void Info(
        string message,
        LogCategory category = LogCategory.Core,
        [CallerMemberName] string caller = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
        => Instance.LogWithCategory(LogLevel.Info, category, message, caller, filePath, lineNumber);

    [Conditional("LOGGING")]
    public static void ExtraInfo(
        string message,
        LogCategory category = LogCategory.Core,
        [CallerMemberName] string caller = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
        => Instance.LogWithCategory(LogLevel.ExtraInfo, category, message, caller, filePath, lineNumber);

    [Conditional("LOGGING")]
    public static void Warning(
        string message,
        LogCategory category = LogCategory.Core,
        [CallerMemberName] string caller = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
        => Instance.LogWithCategory(LogLevel.Warning, category, message, caller, filePath, lineNumber);

    [Conditional("LOGGING")]
    public static void Warn(
        string message,
        LogCategory category = LogCategory.Core,
        [CallerMemberName] string caller = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
        => Instance.LogWithCategory(LogLevel.Warning, category, message, caller, filePath, lineNumber);

    [Conditional("LOGGING")]
    public static void Error(
        string message,
        LogCategory category = LogCategory.Core,
        [CallerMemberName] string caller = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
        => Instance.LogWithCategory(LogLevel.Error, category, message, caller, filePath, lineNumber);

    [Conditional("LOGGING")]
    public static void Critical(
        string message,
        LogCategory category = LogCategory.Core,
        [CallerMemberName] string caller = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
        => Instance.LogWithCategory(LogLevel.Critical, category, message, caller, filePath, lineNumber);

    /// <summary>
    /// Logs message, shutdowns the logger and exits the application with the given exit code. (Default value is 1).
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="category">The category of the log.</param>
    /// <param name="caller">The name of the caller.</param>
    /// <param name="filePath">The path to the file.</param>
    /// <param name="lineNumber">The line number.</param>
    /// <param name="exitCode">The exit code.</param>
    // [Conditional("DEBUG")]
    public static void Fatal(
        string message,
        LogCategory category = LogCategory.Core,
        [CallerMemberName] string caller = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0,
        int exitCode = 1)
    {
        Instance.LogWithCategory(LogLevel.Fatal, category, message, caller, filePath, lineNumber);
        Shutdown();
        Environment.Exit(exitCode);
    }

    // [Conditional("RELEASE")]
    // public static void Fatal(
    //     string message,
    //     int exitCode = 1)
    // {
    //     Shutdown();
    //     Environment.Exit(exitCode);
    // }

    [Conditional("LOGGING")]
    public static void Line()
        => Instance.Log(LogLevel.Line, "=".PadRight(80, '='));

    [Conditional("LOGGING")]
    public static void Assert(bool condition, string message)
    {
        if (!condition)
        {
            Error(message);
        }
    }

    // Internal method that handles category filtering and caller info
    private void LogWithCategory(LogLevel level, LogCategory category, string message, string caller, string filePath,
        int lineNumber)
    {
#if LOGGING
        // Check if category is enabled (null = all categories enabled)
        if (_enabledCategories != null && !_enabledCategories.Contains(category))
        {
            return; // Category filtered out
        }

        // Format message with caller info if enabled
        string finalMessage = message;

        if (_includeCallerInfo && !string.IsNullOrEmpty(caller))
        {
            // Extract class name from file path
            string className = Path.GetFileNameWithoutExtension(filePath);

            // Format: ClassName:Line (simple and clean)
            finalMessage = $"[{className}:{lineNumber}] {message}";
        }

        Log(level, finalMessage);
#endif
    }

    public void Log(LogLevel level, string message)
    {
#if LOGGING
        if (level < _minimumLevel)
        {
            return;
        }

        lock (_lock)
        {
            string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            string levelStr = level.ToString().ToUpper().PadRight(_levelPadding);
            string formattedMessage = $"[{timestamp}] [{levelStr}] {message}";

            // Write to console
            if (_writeToConsole)
            {
                if (_useColors)
                {
                    WriteColoredConsole(level, formattedMessage);
                }
                else
                {
                    Console.WriteLine(formattedMessage);
                }
            }

            // Write to file
            if (_writeToFile && _fileWriter != null)
            {
                try
                {
                    _fileWriter.WriteLine(formattedMessage);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Logger] Failed to write to log file: {ex.Message}");
                }
            }
        }
#endif
    }

    private void WriteColoredConsole(LogLevel level, string message)
    {
        ConsoleColor originalColor = Console.ForegroundColor;

        Console.ForegroundColor = level switch
        {
            LogLevel.Debug => ConsoleColor.Blue,
            LogLevel.Info => ConsoleColor.White,
            LogLevel.Warning => ConsoleColor.Yellow,
            LogLevel.Error => ConsoleColor.Red,
            LogLevel.Fatal => ConsoleColor.DarkRed,
            LogLevel.Critical => ConsoleColor.DarkRed,
            LogLevel.ExtraInfo => ConsoleColor.DarkGray,
            // LogLevel.Line => ConsoleColor.White,
            _ => ConsoleColor.White
        };

        Console.WriteLine(message);
        Console.ForegroundColor = originalColor;
    }

    public string? GetLogFilePath() => _logFilePath;
}
