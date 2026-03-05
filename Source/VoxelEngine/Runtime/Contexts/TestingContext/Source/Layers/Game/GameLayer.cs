using VoxelEngine.Core;
using VoxelEngine.Diagnostics;

namespace VoxelEngine.Runtime.Contexts.TestingContext;

internal sealed class GameLayer : ILayer
{
    private readonly TestRuntimeContext _context;

    public GameLayer(TestRuntimeContext context)
    {
        _context = context;
    }

    public void Initialize()
    {
        Logger.Info("GameLayer.Initialize was called!");
    }

    public void Shutdown()
    {

    }

}
