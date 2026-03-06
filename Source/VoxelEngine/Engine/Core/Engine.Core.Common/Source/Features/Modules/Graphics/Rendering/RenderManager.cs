using System.Numerics;
using VoxelEngine.Core;
using VoxelEngine.Graphics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace VoxelEngine.Graphics.Rendering;

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

        _renderGraph.CameraBuffer = device.Factory.CreateBuffer(new BufferDescription()
        {
            Size = (uint)Unsafe.SizeOf<CameraData>(),
            Usage = BufferUsage.UniformBuffer
        });
    }

    public void Initialize()
    {
        _renderGraph.Initialize();
    }

    public void Submit(RenderCommand renderCommand) => _renderCommands.Add(renderCommand);
    public void Submit(ReadOnlySpan<RenderCommand> renderCommands) => _renderCommands.AddRange(renderCommands);

    public void Render()
    {
        // 1. Process active main camera and flush the UBO
        if (_camerasRegistries.Cameras.Count > 0)
        {
            var activeCamera = _camerasRegistries.Cameras.First();
            var cmdList = device.Factory.CreateCommandsList();
            cmdList.Begin();
            cmdList.UpdateBuffer(_renderGraph.CameraBuffer, 0, ref activeCamera);
            cmdList.BindUniformBuffer(_renderGraph.CameraBuffer, 0); // Slot 0
            cmdList.End();
            device.Submit(cmdList);
        }

        // 3. Dispatch graph
        _renderGraph.Execute(CollectionsMarshal.AsSpan(_renderCommands));

        device.Render();
        device.Present();

        _renderCommands.Clear();
        _camerasRegistries.Clear();
    }

}
