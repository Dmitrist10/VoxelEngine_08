using System.Numerics;
using System.Runtime.CompilerServices;

using Veldrid;

using VoxelEngine.Core;
using VoxelEngine.Diagnostics;

// Alias for disambiguation: both VoxelEngine.Core and Veldrid define PrimitiveTopology
using VoxelPrimTopo = VoxelEngine.Core.PrimitiveTopology;

namespace VoxelEngine.Graphics.Veldrid;

/// <summary>
/// Records rendering commands into a batch and replays them via a Veldrid
/// <see cref="CommandList"/> when <see cref="Execute"/> is called.
///
/// The command encoding is the same byte-stream approach as the OpenGL
/// implementation — this keeps the recording side allocation-free and makes
/// it easy to compare the two backends side-by-side.
/// </summary>
internal sealed unsafe class VEL_GraphicsCommandsList : IGraphicsCommandsList
{
    // -------------------------------------------------------------------------
    // Command opcodes (same set as GL backend — extend as needed)
    // -------------------------------------------------------------------------
    private enum CmdType : byte
    {
        BindPipeline,
        BindMesh,
        PushConstants,
        Draw,
        DrawIndexed,
        BindUniformBuffer,
        UpdateBuffer,
        ClearColor,
        Clear,
    }

    private record struct DrawIndexedCommand(uint IndexCount, VoxelPrimTopo Topology);
    private record struct BindUniformBufferCommand(BufferHandle Buffer, uint BindingSlot);
    private record struct BindBufferCommand(BufferHandle Buffer, uint BindingSlot, uint Offset, uint Size);

    // -------------------------------------------------------------------------
    // Fields
    // -------------------------------------------------------------------------

    private readonly CommandList _cl;
    private readonly VEL_AssetsManager _assets;

    private byte[] _buffer;
    private int _writeOffset;

    public VEL_GraphicsCommandsList(CommandList cl, VEL_AssetsManager assets, int bufferSize = 2 * 1024 * 1024)
    {
        _cl = cl;
        _assets = assets;
        _buffer = new byte[bufferSize];
    }

    // -------------------------------------------------------------------------
    // Write helpers
    // -------------------------------------------------------------------------

    [MethodImpl(AggressiveInlining)]
    private void Write<T>(CmdType type, ref T data) where T : unmanaged
    {
        _buffer[_writeOffset++] = (byte)type;
        Unsafe.WriteUnaligned(ref _buffer[_writeOffset], data);
        _writeOffset += sizeof(T);  // sizeof(T) is fine for stack-only generics
    }

    [MethodImpl(AggressiveInlining)]
    private void Write(CmdType type) => _buffer[_writeOffset++] = (byte)type;

    // -------------------------------------------------------------------------
    // IGraphicsCommandsList recording API (identical surface to GL backend)
    // -------------------------------------------------------------------------

    public void Begin() => _writeOffset = 0;
    public void End() { }

    public void ClearColor(Color color) => Write(CmdType.ClearColor, ref color);
    public void Clear() => Write(CmdType.Clear);

    public void BindPipeline(PipelineHandle pipeline) => Write(CmdType.BindPipeline, ref pipeline);
    public void BindMesh(MeshHandle mesh) => Write(CmdType.BindMesh, ref mesh);

    public void BindTexture(TextureHandle texture, uint slot = 0)
    {
        throw new NotImplementedException();
    }

    public void BindUniformBuffer(BufferHandle buffer, uint bindingSlot = 0)
    {
        var cmd = new BindUniformBufferCommand(buffer, bindingSlot);
        Write(CmdType.BindUniformBuffer, ref cmd);
    }

    public void UpdateBuffer<T>(BufferHandle buffer, uint offset, ref T data) where T : unmanaged
    {
        int tSize = Unsafe.SizeOf<T>();
        BindBufferCommand cmd = new(buffer, Material.MATERIAL_BINDING_SLOT, offset, (uint)tSize);
        Write(CmdType.UpdateBuffer, ref cmd);
        Unsafe.WriteUnaligned(ref _buffer[_writeOffset], data);
        _writeOffset += tSize;
    }

    public void DrawIndexed(uint indexCount, VoxelPrimTopo topology = VoxelPrimTopo.Triangles)
        => Write(CmdType.DrawIndexed, ref indexCount);

    // -------------------------------------------------------------------------
    // Playback
    // -------------------------------------------------------------------------

    /// <summary>Replays every recorded command through the Veldrid command list.</summary>
    public void Execute(GraphicsDevice device)
    {
        _cl.Begin();

        int readOffset = 0;

        fixed (byte* pBuffer = _buffer)
        {
            while (readOffset < _writeOffset)
            {
                CmdType cmd = (CmdType)pBuffer[readOffset++];

                switch (cmd)
                {
                    // ---------------------------------------------------------
                    case CmdType.ClearColor:
                        {
                            var color = Unsafe.ReadUnaligned<Color>(pBuffer + readOffset);
                            readOffset += sizeof(Color);

                            RgbaFloat clearColor = new(color.R, color.G, color.B, color.A);
                            _cl.SetFramebuffer(device.SwapchainFramebuffer);
                            _cl.ClearColorTarget(0, clearColor);
                            _cl.ClearDepthStencil(1f);
                            break;
                        }
                    // ---------------------------------------------------------
                    case CmdType.Clear:
                        {
                            _cl.SetFramebuffer(device.SwapchainFramebuffer);
                            _cl.ClearColorTarget(0, RgbaFloat.Black);
                            _cl.ClearDepthStencil(1f);
                            break;
                        }
                    // ---------------------------------------------------------
                    case CmdType.BindPipeline:
                        {
                            var handle = Unsafe.ReadUnaligned<PipelineHandle>(pBuffer + readOffset);
                            readOffset += sizeof(PipelineHandle);
                            _cl.SetPipeline(_assets.Get(handle).Pipeline);
                            break;
                        }
                    // ---------------------------------------------------------
                    case CmdType.BindMesh:
                        {
                            var handle = Unsafe.ReadUnaligned<MeshHandle>(pBuffer + readOffset);
                            readOffset += sizeof(MeshHandle);
                            VEL_Mesh mesh = _assets.Get(handle);
                            _cl.SetVertexBuffer(0, mesh.VertexBuffer);
                            _cl.SetIndexBuffer(mesh.IndexBuffer, IndexFormat.UInt32);
                            break;
                        }
                    // ---------------------------------------------------------
                    case CmdType.DrawIndexed:
                        {
                            var indexCount = Unsafe.ReadUnaligned<uint>(pBuffer + readOffset);
                            readOffset += sizeof(uint);
                            _cl.DrawIndexed(indexCount);
                            break;
                        }
                    // ---------------------------------------------------------
                    case CmdType.BindUniformBuffer:
                        {
                            // TODO: wire up Veldrid ResourceSets for uniform buffers
                            readOffset += sizeof(BindUniformBufferCommand);
                            break;
                        }
                    // ---------------------------------------------------------
                    case CmdType.UpdateBuffer:
                        {
                            var updateCmd = Unsafe.ReadUnaligned<BindBufferCommand>(pBuffer + readOffset);
                            readOffset += sizeof(BindBufferCommand);

                            VEL_Buffer velBuffer = _assets.Get(updateCmd.Buffer);
                            _device_UpdateBuffer(device, velBuffer.Buffer, updateCmd.Offset, pBuffer + readOffset, updateCmd.Size);
                            readOffset += (int)updateCmd.Size;
                            break;
                        }
                    // ---------------------------------------------------------
                    default:
                        Logger.Error("[Veldrid] Unknown command type: " + cmd);
                        goto done;
                }
            }
        }

    done:
        _cl.End();
        device.SubmitCommands(_cl);
    }

    // Small helper to call device.UpdateBuffer with a raw pointer
    [MethodImpl(AggressiveInlining)]
    private static void _device_UpdateBuffer(GraphicsDevice device, DeviceBuffer buffer, uint offset, byte* data, uint size)
    {
        device.UpdateBuffer(buffer, offset, (nint)data, size);
    }

    public void Dispose() => _cl.Dispose();
}