using VoxelEngine.Core;
using VoxelEngine.Graphics;

namespace VoxelEngine.Graphics.Rendering;

public sealed class RenderGraph
{

    private List<RenderPass> _renderPasses;

    private IGraphicsDevice device = null!;

    public BufferHandle CameraBuffer;

    public RenderGraph()
    {
        _renderPasses = new List<RenderPass>();
        _renderPasses.Add(new RP_ClearColor());
        _renderPasses.Add(new RP_Forward());
    }

    public void Initialize()
    {
        device = ServiceContainer.Get<GraphicsContext>()!.Device;

        foreach (var renderPass in _renderPasses)
        {
            renderPass.Init(device, this);
            renderPass.Initialize();
        }
    }

    public void Execute(ReadOnlySpan<RenderCommand> renderCommands)
    {
        foreach (var renderPass in _renderPasses)
        {
            renderPass.Execute(renderCommands);
        }
    }

}
