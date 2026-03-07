using VoxelEngine.Diagnostics;
using VoxelEngine.Core;
using VoxelEngine.Core.UGC;
using VoxelEngine.Client;
using VoxelEngine.Server;

namespace VoxelEngine.Runtime.Contexts.TestingContext;

public sealed class TestRuntimeContext : IRuntimeContext
{

    private readonly ClientHost _clientHost;
    private readonly ServerHost _serverHost;

    private readonly GameLoader _gameLoader;
    private readonly LayerManager _layerManager;


    public TestRuntimeContext()
    {
        Logger.Info("TestRuntimeContext created");

        _clientHost = new ClientHost();
        _serverHost = new ServerHost();
        _gameLoader = new GameLoader();

        _layerManager = new LayerManager();
    }

    public void Init()
    {
        Logger.Info("TestRuntimeContext Initialized");

        _clientHost.Initialize();
        _serverHost.Initialize();
        _layerManager.SetLayer(new LauncherLayer(this));
    }

    public void StartSinglePlayer(string gamePath)
    {
        LoadGame(gamePath);
        _layerManager.SetLayer(new GameLayer(this));
    }

    public void LoadGame(string path)
    {
        try
        {
            var game = _gameLoader.LoadGame(path);
            if (game == null)
                throw new NullReferenceException("Game is null");

            game.SetUp(_serverHost.UniverseManager);
            game.OnInitialize();
            game.StartMatch(); // for now 
        }
        catch (Exception e)
        {
            Logger.Error($"Your game crashed! {e.Message} stacktrace:{e.StackTrace}");
        }
    }

    public void Update()
    {
        _serverHost.Update();
    }

    public void FixedUpdate()
    {
        _serverHost.FixedUpdate();
    }
    public void Render()
    {
        _serverHost.Render();
        _clientHost.Render();
    }

    public void Tick()
    {
        _serverHost.Tick();
    }

    public void Cleanup()
    {
    }

}