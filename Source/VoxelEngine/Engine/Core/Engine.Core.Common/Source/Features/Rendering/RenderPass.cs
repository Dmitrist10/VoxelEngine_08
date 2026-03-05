using VoxelEngine.Graphics;

namespace VoxelEngine.Core.Rendering;

internal abstract class RenderPass
{
    protected IGraphicsDevice device = null!;
    protected RenderGraph renderGraph = null!;
    protected IGraphicsFactory factory = null!;

    internal void Init(IGraphicsDevice device, RenderGraph renderGraph)
    {
        this.device = device;
        this.renderGraph = renderGraph;
        this.factory = device.Factory;
    }

    internal abstract void Initialize();
    internal abstract void Execute(ReadOnlySpan<RenderCommand> renderCommands);
}
