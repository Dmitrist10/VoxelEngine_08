using VoxelEngine.Graphics;
using VoxelEngine.Core;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace VoxelEngine.Core.Rendering;

internal sealed class RP_Forward : RenderPass
{
    private const uint MODEL_BINDING_SLOT = 2;

    private BufferHandle _modelBuffer;

    internal override void Initialize()
    {
        _modelBuffer = factory.CreateBuffer(new BufferDescription()
        {
            Size = (uint)Unsafe.SizeOf<Matrix4x4>(),
            Usage = BufferUsage.UniformBuffer
        });
    }

    internal override void Execute(ReadOnlySpan<RenderCommand> renderCommands)
    {
        IGraphicsCommandsList commandList = factory.CreateCommandsList();

        commandList.Begin();

        foreach (var renderCommand in renderCommands)
        {
            // Upload this entity's world matrix to the model UBO
            var worldMatrix = renderCommand.Transform;
            commandList.UpdateBuffer(_modelBuffer, 0, ref worldMatrix);
            commandList.BindUniformBuffer(_modelBuffer, MODEL_BINDING_SLOT);

            renderCommand.Material.SetRendering(commandList);
            commandList.BindMesh(renderCommand.Mesh.Handle);
            commandList.DrawIndexed(renderCommand.Mesh.IndexCount);
        }

        commandList.End();

        device.Submit(commandList);
    }

}
