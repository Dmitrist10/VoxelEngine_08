using VoxelEngine.Graphics;
using VoxelEngine.Core;
using System.Diagnostics;

namespace VoxelEngine.Client.Rendering;

internal sealed class RP_Forward : RenderPass
{
    internal override void Initialize()
    {
        // Now handled by RenderManager initialization for testing bounds
    }

    internal override void Execute(ReadOnlySpan<RenderCommand> renderCommands)
    {
        IGraphicsCommandsList commandList = factory.CreateCommandsList();

        commandList.Begin();

        foreach (var renderCommand in renderCommands)
        {
            renderCommand.Material.SetRendering(commandList);
            commandList.BindMesh(renderCommand.Mesh.Handle);
            // Ignore transform matrix for now until Model UBO is added
            commandList.DrawIndexed(renderCommand.Mesh.IndexCount);
        }

        commandList.End();

        device.Submit(commandList);
    }

}