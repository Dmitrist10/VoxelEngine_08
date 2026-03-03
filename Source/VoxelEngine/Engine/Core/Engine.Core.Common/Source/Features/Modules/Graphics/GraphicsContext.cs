namespace VoxelEngine.Graphics;

public sealed class GraphicsContext
{
    private IWindowSurface window;
    public IWindowSurface Window => window;

    private IGraphicsDevice device;
    public IGraphicsDevice Device => device;

    public GraphicsContext(IWindowSurface window, IGraphicsDevice device)
    {
        this.window = window;
        this.device = device;
    }

}
