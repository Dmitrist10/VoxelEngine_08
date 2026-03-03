namespace VoxelEngine.Graphics;

public interface IGraphicsFactory : IDisposable
{
    IGraphicsCommandsList CreateCommandsList();
}
