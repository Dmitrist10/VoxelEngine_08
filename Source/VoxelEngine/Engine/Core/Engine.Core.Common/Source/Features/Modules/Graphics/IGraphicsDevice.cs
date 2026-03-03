namespace VoxelEngine.Graphics;

public interface IGraphicsDevice : IDisposable
{
    IGraphicsFactory Factory { get; }

    void Submit(IGraphicsCommandsList list);
    void Submit(ReadOnlySpan<IGraphicsCommandsList> lists);

    void Render();
    void Present();
}
