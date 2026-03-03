using Silk.NET.OpenGL;
using VoxelEngine.Core;

namespace VoxelEngine.Graphics.OpenGL;

internal unsafe class GL_GraphicsCommandsList : IGraphicsCommandsList
{
    private enum CmdType : byte
    {
        BindPipeline,
        BindMesh,
        PushConstants,
        DrawIndexed,
        BindTexture,
        BindUniformBuffer,
        ClearColor,
        Clear
    }

    private readonly GL _GL;

    private byte[] _buffer;
    private int _writeOffset;

    public GL_GraphicsCommandsList(GL gL, int bufferSize = 2 * 1024 * 1024) // 2MB
    {
        _GL = gL;
        _buffer = new byte[bufferSize];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Write<T>(CmdType type, ref T data) where T : unmanaged
    {
        _buffer[_writeOffset++] = (byte)type;
        Unsafe.WriteUnaligned(ref _buffer[_writeOffset], data);
        _writeOffset += sizeof(T);
    }


    public void Begin()
    {
        _writeOffset = 0;
    }
    public void End()
    {
    }

    public void ClearColor(Color color)
    {
        Write(CmdType.ClearColor, ref color);
    }

    public void Execute()
    {
        int readOffset = 0;

        // Pin the array so the Garbage Collector doesn't move it while we read
        fixed (byte* pBuffer = _buffer)
        {
            while (readOffset < _writeOffset)
            {
                // Read 1 byte for the command type
                CmdType cmd = (CmdType)pBuffer[readOffset++];

                switch (cmd)
                {
                    case CmdType.ClearColor:
                        var color = Unsafe.ReadUnaligned<Color>(pBuffer + readOffset);
                        readOffset += sizeof(Color);
                        _GL.ClearColor(color.R, color.G, color.B, color.A);
                        _GL.Clear((uint)ClearBufferMask.ColorBufferBit);
                        break;
                }
            }
        }
    }

    public void Dispose()
    {
    }

}