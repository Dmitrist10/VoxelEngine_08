namespace VoxelEngine.Diagnostics;

public class LoggerConfig
{
    public LogLevel MinimumLevel { get; set; } = LogLevel.Debug;
    public bool WriteToConsole { get; set; } = true;
    public bool WriteToFile { get; set; } = true;
    public bool UseColors { get; set; } = true;
    public bool IncludeCallerInfo { get; set; } = true;
    public LogCategory[]? EnabledCategories { get; set; } = null; // null = all categories enabled
    public string? LogDirectory { get; set; } = "logs";
    public string? FileNamePattern { get; set; } = "engine_{timestamp}.log";
    public int LevelPadding { get; set; } = 5;
}
