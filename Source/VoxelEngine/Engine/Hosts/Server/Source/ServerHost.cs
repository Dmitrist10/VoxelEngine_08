using VoxelEngine.Core;
using System.Text;

namespace VoxelEngine.Server;

public class ServerHost : IHostBase
{
    public UniverseManager UniverseManager { get; }

    public ServerHost()
    {
        UniverseManager = new UniverseManager();
    }

    public void Initialize()
    {
    }



    public void Render()
    {
        UniverseManager.Render();
    }

    public void Tick()
    {
        UniverseManager.Tick();
    }

    public void FixedUpdate()
    {
        UniverseManager.FixedUpdate();
    }

    public void Update()
    {
        UniverseManager.Update();
    }

}