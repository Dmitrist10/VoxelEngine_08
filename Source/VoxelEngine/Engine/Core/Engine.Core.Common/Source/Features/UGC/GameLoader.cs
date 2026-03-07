using VoxelEngine.Core;
using VoxelEngine.Core.UGC;
using VoxelEngine.Diagnostics;

namespace VoxelEngine.Core.UGC;

public sealed class GameLoader
{
    private IGame? _currentGame;

    public void Initialize()
    {
    }

    public IGame? LoadGame(string path)
    {
        try
        {
            Logger.Info($"Loading game from {path}");
            LoadGameDll(path);
        }
        catch (Exception e)
        {
            Logger.Error($"Loading Game Failed! {e.Message} stacktrace:{e.StackTrace}");
        }

        return _currentGame;
    }

    private void LoadGameDll(string path)
    {
        // 1. Load Assembly
        var assembly = System.Reflection.Assembly.LoadFrom(path);

        // 2. Find IGame implementation
        var gameType = assembly.GetTypes()
            .FirstOrDefault(t => typeof(GameBase).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

        if (gameType == null)
        {
            Logger.Error("No GameBase found in " + path);
            return;
        }

        // 3. Instantiate
        _currentGame = (IGame)Activator.CreateInstance(gameType)!;
    }


}