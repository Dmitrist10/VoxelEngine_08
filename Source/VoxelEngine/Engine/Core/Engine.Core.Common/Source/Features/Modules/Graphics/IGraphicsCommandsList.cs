using VoxelEngine.Core;

namespace VoxelEngine.Graphics;

public interface IGraphicsCommandsList : IDisposable
{
    void Begin();
    void End();

    void ClearColor(Color color);
}