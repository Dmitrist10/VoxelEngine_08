using VoxelEngine.Core;
using VoxelEngine.Diagnostics;
using VoxelEngine.Graphics;

namespace VoxelEngine.Client.Rendering;

internal sealed class RP_ClearColor : RenderPass
{
    internal override void Initialize()
    {
    }

    internal override void Execute(ReadOnlySpan<RenderCommand> renderCommands)
    {
        IGraphicsCommandsList commandList = factory.CreateCommandsList();

        commandList.Begin();
        commandList.ClearColor(Color.White * 0.15f);
        commandList.End();

        device.Submit(commandList);
    }

}