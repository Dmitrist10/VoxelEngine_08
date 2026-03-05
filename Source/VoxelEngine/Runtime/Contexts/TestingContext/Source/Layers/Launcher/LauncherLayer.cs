using VoxelEngine.Core;
using VoxelEngine.Diagnostics;

namespace VoxelEngine.Runtime.Contexts.TestingContext;

internal sealed class LauncherLayer : ILayer
{
    private TestRuntimeContext _context = null!;

    public LauncherLayer(TestRuntimeContext context)
    {
        _context = context;
    }

    public void Initialize()
    {
        Logger.Info("LauncherLayer.Initialize was called!");
        _context.StartSinglePlayer(@"C:\Users\Dmitrist10\Desktop\VoxelGames\Source\Project_08_VE\Source\Resources\Games\Game_01\Main\bin\net11.0\Game_01.dll");
    }

    public void Shutdown()
    {

    }

}