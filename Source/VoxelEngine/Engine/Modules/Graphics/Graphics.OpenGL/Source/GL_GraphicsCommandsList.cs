using Silk.NET.OpenGL;
using VoxelEngine.Core;
using VoxelEngine.Diagnostics;

namespace VoxelEngine.Graphics.OpenGL;

internal unsafe class GL_GraphicsCommandsList : IGraphicsCommandsList
{
    private enum CmdType : byte
    {
        BindPipeline,
        BindMesh,
        PushConstants,
        Draw,
        DrawIndexed,
        BindTexture,
        BindUniformBuffer,
        UpdateBuffer,
        ClearColor,
        Clear
    }

    private record struct DrawIndexedCommand(uint IndexCount, PrimitiveTopology Topology);
    private record struct BindTextureCommand(TextureHandle Texture, uint Slot);
    private record struct BindUniformBufferCommand(BufferHandle Buffer, uint BindingSlot);
    private record struct BindBufferCommand(BufferHandle Buffer, uint BindingSlot, uint Offset, uint Size);

    private readonly GL _GL;

    private byte[] _buffer;
    private int _writeOffset;

    private GL_AssetsManager _assetsManager;

    public GL_GraphicsCommandsList(GL gL, GL_AssetsManager manager, int bufferSize = 2 * 1024 * 1024) // 2MB
    {
        _GL = gL;
        _assetsManager = manager;
        _buffer = new byte[bufferSize];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Write<T>(CmdType type, ref T data) where T : unmanaged
    {
        _buffer[_writeOffset++] = (byte)type;
        Unsafe.WriteUnaligned(ref _buffer[_writeOffset], data);
        _writeOffset += sizeof(T);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Write(CmdType type)
    {
        _buffer[_writeOffset++] = (byte)type;
    }


    public void Begin()
    {
        _writeOffset = 0;
    }
    public void End()
    {
    }

    public void ClearColor(Color color) => Write(CmdType.ClearColor, ref color);
    public void Clear() => Write(CmdType.Clear);


    public void BindPipeline(PipelineHandle pipeline) => Write(CmdType.BindPipeline, ref pipeline);
    public void BindMesh(MeshHandle mesh) => Write(CmdType.BindMesh, ref mesh);
    public void BindTexture(TextureHandle texture, uint slot = 0)
    {
        if (!texture.Handle.IsValid) return; // small safty check

        var cmd = new BindTextureCommand(texture, slot);
        Write(CmdType.BindTexture, ref cmd);
    }
    public void BindUniformBuffer(BufferHandle buffer, uint bindingSlot = 0)
    {
        var cmd = new BindUniformBufferCommand(buffer, bindingSlot);
        Write(CmdType.BindUniformBuffer, ref cmd);
    }

    public void UpdateBuffer<T>(BufferHandle buffer, uint offset, ref T data) where T : unmanaged
    {
        BindBufferCommand bindBufferCommand = new(buffer, Material.MATERIAL_BINDING_SLOT, offset, (uint)sizeof(T));
        Write(CmdType.UpdateBuffer, ref bindBufferCommand);
        Unsafe.WriteUnaligned(ref _buffer[_writeOffset], data);
        _writeOffset += sizeof(T);
    }

    // public void Draw(uint vertexCount) => Write(CmdType.Draw, ref vertexCount);
    public void DrawIndexed(uint indexCount, PrimitiveTopology topology = PrimitiveTopology.Triangles) => Write(CmdType.DrawIndexed, ref indexCount);


    // [MethodImpl(AggressiveInlining)]
    // private PrimitiveType GetGLTopology(PrimitiveTopology topology)
    // {
    //     return topology switch
    //     {
    //         PrimitiveTopology.Triangles => PrimitiveType.Triangles,
    //         PrimitiveTopology.Lines => PrimitiveType.Lines,
    //         PrimitiveTopology.LineStrip => PrimitiveType.LineStrip,
    //         _ => PrimitiveType.Triangles
    //     };
    // }

    public void Execute()
    {
        try
        {

            int readOffset = 0;

            // prevent GC from moving the byte[]
            fixed (byte* pBuffer = _buffer)
            {
                while (readOffset < _writeOffset)
                {
                    // Read 1 byte for the command type
                    CmdType cmd = (CmdType)pBuffer[readOffset++];

                    switch (cmd)
                    {
                        case CmdType.ClearColor:
                            E_ClearColor(ref readOffset, pBuffer);
                            break;
                        case CmdType.Clear:
                            _GL.Clear((uint)(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit));
                            break;

                        case CmdType.BindPipeline:
                            var pipeline = Unsafe.ReadUnaligned<PipelineHandle>(pBuffer + readOffset);
                            readOffset += sizeof(PipelineHandle);
                            // _GL.BindProgramPipeline(_assetsManager.Get(pipeline).ID);
                            _GL.UseProgram(_assetsManager.Get(pipeline).ID);
                            break;
                        case CmdType.BindMesh:
                            var mesh = Unsafe.ReadUnaligned<MeshHandle>(pBuffer + readOffset);
                            readOffset += sizeof(MeshHandle);
                            // GL_Mesh glMesh = _assetsManager.Get(mesh);
                            _GL.BindVertexArray(_assetsManager.Get(mesh).VAO);
                            break;
                        case CmdType.BindTexture:
                            // TODO: Right now if no texture assigned then it uses the last binded
                            var bindTextureCmd = Unsafe.ReadUnaligned<BindTextureCommand>(pBuffer + readOffset);
                            readOffset += sizeof(BindTextureCommand);
                            GL_TextureResource glTexture = _assetsManager.Get(bindTextureCmd.Texture);
                            var textureTarget = glTexture.Type == GL_TextureType.Texture2DArray
                                ? TextureTarget.Texture2DArray
                                : TextureTarget.Texture2D;
                            _GL.ActiveTexture((TextureUnit)((uint)TextureUnit.Texture0 + bindTextureCmd.Slot));
                            _GL.BindTexture(textureTarget, glTexture.ID);
                            break;
                        case CmdType.BindUniformBuffer:
                            var bindUniformCmd = Unsafe.ReadUnaligned<BindUniformBufferCommand>(pBuffer + readOffset);
                            readOffset += sizeof(BindUniformBufferCommand);
                            GL_Buffer glBuffer = _assetsManager.Get(bindUniformCmd.Buffer);
                            _GL.BindBufferBase(BufferTargetARB.UniformBuffer, bindUniformCmd.BindingSlot, glBuffer.ID);
                            break;
                        case CmdType.UpdateBuffer:
                            var updateBufferCommand = Unsafe.ReadUnaligned<BindBufferCommand>(pBuffer + readOffset);
                            readOffset += sizeof(BindBufferCommand);

                            GL_Buffer glUpdateBuffer = _assetsManager.Get(updateBufferCommand.Buffer);
                            _GL.BindBuffer(BufferTargetARB.UniformBuffer, glUpdateBuffer.ID);
                            _GL.BufferSubData(BufferTargetARB.UniformBuffer, (nint)updateBufferCommand.Offset, (nuint)updateBufferCommand.Size, pBuffer + readOffset);

                            readOffset += (int)updateBufferCommand.Size;
                            break;
                        case CmdType.Draw:
                            var vertexCount = Unsafe.ReadUnaligned<uint>(pBuffer + readOffset);
                            readOffset += sizeof(uint);
                            _GL.DrawArrays(PrimitiveType.Triangles, 0, vertexCount);
                            break;
                        case CmdType.DrawIndexed:
                            var indexCount = Unsafe.ReadUnaligned<uint>(pBuffer + readOffset);
                            readOffset += sizeof(uint);
                            _GL.DrawElements(PrimitiveType.Triangles, indexCount, DrawElementsType.UnsignedInt, (void*)0);
                            break;
                        default:
                            Logger.Error("Unknown command type: " + cmd);
                            return;
                    }
                }
            }
        }
        catch (Exception e)
        {
            Logger.Error("Crashed while executing rendering commands stacktrace: " + e.StackTrace);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void E_ClearColor(ref int readOffset, byte* pBuffer)
    {
        var color = Unsafe.ReadUnaligned<Color>(pBuffer + readOffset);
        readOffset += sizeof(Color);
        _GL.ClearColor(color.R, color.G, color.B, color.A);
        _GL.Clear((uint)(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit));
    }

    public void Dispose()
    {
    }

}