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

    public void OnInitialize()
    {


    }

    // public void OnMatchStart()
    public void StartGameSession()
    {
        Universe u = UniverseManager.CreateUniverse();
        Scene s = u.CreateScene();

        // s.SetActive(true);
        GameSetup.SetUp(s);
    }

    public void OnShutDown()
    {


    }

}
