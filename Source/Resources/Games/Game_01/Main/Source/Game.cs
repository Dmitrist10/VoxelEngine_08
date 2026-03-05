using VoxelEngine.Core;
using VoxelEngine.Core.UGC;
using VoxelEngine.Diagnostics;

namespace CustomGame;

public sealed class Game : GameBase
{

    // private GameSessionManager gameSessionManager;

    public Game()
    {
        // gameSessionManager = new GameSessionManager();
    }

    public override void OnInitialize()
    {
        Logger.Info("Game.OnInitialize was called!");
    }

    // public void OnMatchStart()
    public override void StartMatch()
    {
        Logger.Info("Game.StartMatch was called!");
        Universe u = UniverseManager.CreateUniverse();
        Scene s = u.CreateScene();

        GameSetup.SetUp(s);
    }

    public void OnShutDown()
    {


    }

    public static void A()
    {
        Logger.Info("Game.A was called!");
    }
}
