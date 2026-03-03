using VoxelEngine.Client.Rendering;
using VoxelEngine.Core;

namespace VoxelEngine.Client;

public class ClientHost : IHostBase
{

    private RenderManager _renderManager = null!;

    public ClientHost()
    {
    }

    public void Initialize()
    {
        _renderManager = new RenderManager();
        _renderManager.Initialize();
    }

    public void Update()
    {
    }

    public void Render()
    {
        _renderManager.Render();
    }


}
