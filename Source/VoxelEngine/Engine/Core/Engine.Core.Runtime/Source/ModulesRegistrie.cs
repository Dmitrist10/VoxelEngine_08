using VoxelEngine.Graphics;

namespace VoxelEngine.Core.Runtime;

internal sealed class ModulesManager
{
    private List<IEngineModule> _modules = new List<IEngineModule>();

    private IWindowSurface window;

    public ModulesManager()
    {
        window = ServiceContainer.Get<GraphicsContext>()!.Window;
        window.OnLoad += Load;
    }

    public void Register(IEngineModule module)
    {
        _modules.Add(module);
    }

    public void Unregister(IEngineModule module)
    {
        _modules.Remove(module);
    }

    public void Update()
    {
        foreach (var module in _modules)
        {
            module.OnUpdate();
        }
    }
    public void Initialize()
    {
        foreach (var module in _modules)
        {
            module.OnInitialize();
        }
    }
    public void Load()
    {
        foreach (var module in _modules)
        {
            module.OnLoad(window);
        }
        window.OnLoad -= Load;
    }

}