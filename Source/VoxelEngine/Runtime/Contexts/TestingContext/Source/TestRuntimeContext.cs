using VoxelEngine.Client;
using VoxelEngine.Core;
using VoxelEngine.Diagnostics;
using VoxelEngine.Server;

namespace VoxelEngine.Runtime.Contexts.TestingContext;

public sealed class TestRuntimeContext : IRuntimeContext
{

    private ClientHost _clientHost;
    private ServerHost _serverHost;

    public TestRuntimeContext()
    {
        Logger.Info("TestRuntimeContext created");

        _clientHost = new ClientHost();
        _serverHost = new ServerHost();
    }

    public void Init()
    {
        Logger.Info("TestRuntimeContext Initialized");

        _clientHost.Initialize();
        _serverHost.Initialize();
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