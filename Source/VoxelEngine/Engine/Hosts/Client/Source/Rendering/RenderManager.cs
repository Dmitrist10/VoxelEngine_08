using VoxelEngine.Core;
using VoxelEngine.Graphics;

namespace VoxelEngine.Client.Rendering;

public sealed class RenderManager
{
    private readonly RenderGraph _renderGraph;
    private readonly CamerasRegistries _camerasRegistries;

    private List<RenderCommand> _renderCommands = new(10000);
    // public IReadOnlyList<RenderCommand> RenderCommands => _renderCommands;

    private IGraphicsDevice device;

    public RenderManager()
    {
        _renderGraph = new RenderGraph();
        _renderCommands = new List<RenderCommand>();

        _camerasRegistries = new CamerasRegistries();
        ServiceContainer.Register(_camerasRegistries);

        device = ServiceContainer.Get<GraphicsContext>()!.Device;
    }

    internal void Initialize()
    {
        _renderGraph.Initialize();
    }

    internal void Submit(RenderCommand renderCommand) => _renderCommands.Add(renderCommand);
    internal void Submit(ReadOnlySpan<RenderCommand> renderCommands) => _renderCommands.AddRange(renderCommands);

    public void Render()
    {
        _renderGraph.Execute(System.Runtime.InteropServices.CollectionsMarshal.AsSpan(_renderCommands));

        device.Render();
        device.Present();

        _renderCommands.Clear();
    }

}