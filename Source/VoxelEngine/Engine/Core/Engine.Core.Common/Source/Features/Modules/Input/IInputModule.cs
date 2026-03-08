using VoxelEngine.Core;

namespace VoxelEngine.Input;

public interface IInputModule : IEngineModule
{
    IInput GetInput();
}
