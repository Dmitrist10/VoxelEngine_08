using VoxelEngine.Graphics;

namespace VoxelEngine.Client.Rendering;

internal sealed class RP_Forward : RenderPass
{
    internal override void Initialize()
    {
    }

    internal override void Execute(ReadOnlySpan<RenderCommand> renderCommands)
    {
        IGraphicsCommandsList commandList = factory.CreateCommandsList();

        commandList.Begin();

        foreach (var renderCommand in renderCommands)
        {
            
        }

        commandList.End();

        device.Submit(commandList);
    }

}