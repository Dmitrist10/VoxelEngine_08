using VoxelEngine.Client.Rendering;
using VoxelEngine.Core;
using VoxelEngine.Diagnostics;

namespace VoxelEngine.Client;

public class ClientHost : IHostBase
{

    private RenderManager _renderManager = null!;

    public ClientHost()
    {
        UniverseManager.instance.OnSceneCreated += OnSceneLoaded;
    }

    public void Initialize()
    {
        _renderManager = new RenderManager();
        ServiceContainer.Register(_renderManager);
        _renderManager.Initialize();
    }

    private void OnSceneLoaded(Scene scene)
    {
        Logger.Info("Scene loaded");
        scene.AddProcessor(new EP_MeshRenderer());
        scene.AddProcessor(new EP_Camera());
    }

    public void Update()
    {
    }

    public void Render()
    {
        _renderManager.Render();
    }

    ~ClientHost()
    {
        UniverseManager.instance.OnSceneCreated -= OnSceneLoaded;
    }

}
