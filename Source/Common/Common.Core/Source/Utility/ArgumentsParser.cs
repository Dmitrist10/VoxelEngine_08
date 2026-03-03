namespace VoxelEngine.Core;

/// <summary>
/// Parses arguments for a specific key with a value.
/// Example: --key=value
/// </summary>
public static class ArgumentsParser
{
    public static string[] args = Array.Empty<string>();

    /// <summary>
    /// Parses arguments for a specific key with a value.
    /// Example: --key=value
    /// </summary>
    /// <param name="args">Array of arguments.</param>
    /// <param name="result">Result of parsing.</param>
    /// <returns>True if parsing was successful, false otherwise.</returns>
    public static bool TryParse(string[] args, out string result)
    {
        result = string.Empty;

        foreach (var arg in args)
        {
            if (arg.StartsWith("--"))
            {
                var parts = arg.Split('=');
                if (parts.Length == 2)
                {
                    result = parts[1];
                    return true;
                }
            }
        }

        return false;
    }

    public static bool TryParse(out string result)
    {
        result = string.Empty;

        foreach (var arg in args)
        {
            if (arg.StartsWith("--"))
            {
                var parts = arg.Split('=');
                if (parts.Length == 2)
                {
                    result = parts[1];
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Checks if an argument exists in the array.
    /// Example: --key
    /// </summary>
    /// <param name="args">Array of arguments.</param>
    /// <param name="key">Key to check.</param>
    /// <returns>True if the argument exists, false otherwise.</returns>
    public static bool HasArg(string[] args, string key)
    {
        foreach (var arg in args)
        {
            if (arg.StartsWith($"--{key}"))
            {
                return true;
            }
        }

        return false;
    }
    public static bool HasArg(string key)
    {
        foreach (var arg in args)
        {
            if (arg.StartsWith($"--{key}"))
            {
                return true;
            }
        }

        return false;
    }

    public static void Bind_Args(string[] _args)
    {
        args = _args;
    }

}

