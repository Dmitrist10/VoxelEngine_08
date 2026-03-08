using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using VoxelEngine.Core;

namespace VoxelEngine.Packages.Voxel;

public sealed unsafe class Chunk : IDisposable
{
    public const int SIZE = 32;
    public const int VOLUME = SIZE * SIZE * SIZE;
    public const nuint SizeInBytes = (nuint)VOLUME * sizeof(uint);

    private readonly uint* _blocks;
    public Int3 Position { get; internal set; }
    public MeshAsset Mesh { get; internal set; }
    public bool IsDirty { get; private set; } = true;
    private bool _isDisposed;

    public Chunk(Int3 chunkPos, uint[] uints)
    {
        // _blocks = (uint*)System.Runtime.InteropServices.Marshal.AllocHGlobal(SizeInBytes);
        _blocks = (uint*)NativeMemory.Alloc(SizeInBytes);
        Position = chunkPos;

        uints.AsSpan().CopyTo(new Span<uint>(_blocks, VOLUME));
    }
    // public Chunk(Int3 chunkPos, uint* blocks)
    // {
    //     // _blocks = (uint*)System.Runtime.InteropServices.Marshal.AllocHGlobal(SizeInBytes);
    //     // _blocks = (uint*)NativeMemory.Alloc(SizeInBytes);
    //     _blocks = blocks;
    //     Position = chunkPos;

    // }

    [MethodImpl(AggressiveInlining)]
    public static int GetBlockIndex(int x, int y, int z) => x | (y << 5) | (z << 10);
    [MethodImpl(AggressiveInlining)]
    public static int GetBlockIndex(Int3 pos) => pos.X | (pos.Y << 5) | (pos.Z << 10);

    [MethodImpl(AggressiveInlining)]
    public uint GetBlock(int x, int y, int z)
    {
        if ((uint)x >= SIZE || (uint)y >= SIZE || (uint)z >= SIZE) // unsigned to make faster
            return 0; // out of bounds = air

        return _blocks[GetBlockIndex(x, y, z)];
    }
    [MethodImpl(AggressiveInlining)]
    public uint GetBlock(Int3 pos)
    {
        if ((uint)pos.X >= SIZE || (uint)pos.Y >= SIZE || (uint)pos.Z >= SIZE)
            return 0; // out of bounds = air

        return _blocks[GetBlockIndex(pos)];
    }
    [MethodImpl(AggressiveInlining)]
    public uint GetBlockUnsafe(Int3 pos) => _blocks[GetBlockIndex(pos)];
    public uint GetBlockUnsafe(int x, int y, int z) => _blocks[GetBlockIndex(x, y, z)];

    [MethodImpl(AggressiveInlining)]
    public void SetBlock(int x, int y, int z, uint block)
    {
        _blocks[GetBlockIndex(x, y, z)] = block;
        IsDirty = true;
    }
    [MethodImpl(AggressiveInlining)]
    public void SetBlock(Int3 pos, uint block)
    {
        _blocks[GetBlockIndex(pos)] = block;
        IsDirty = true;
    }

    [MethodImpl(AggressiveInlining)]
    public void ClearDirty() => IsDirty = false;

    // [MethodImpl(AggressiveInlining)]
    // public void SetBlocks(in uint* blocks) => _blocks = blocks;

    public Span<uint> GetBlocksAsSpan()
    {
        return new Span<uint>(_blocks, VOLUME);
    }


    // public bool TryGetMesh([NotNullWhen(true)] out MeshAsset? mesh)
    // {
    //     if (Mesh == null)
    //     {
    //         mesh = default;
    //         return false;
    //     }

    //     mesh = Mesh.Value;
    //     return true;
    // }

    // public ref uint[] GetBlocks()
    // {
    //     return ref blocks;
    // }


    public void Dispose()
    {
        // if (_isDisposed)
        //     return;
        // _isDisposed = true;

        NativeMemory.Free(_blocks);
        // GC.SuppressFinalize(this);
    }

    // ~Chunk()
    // {
    //     Dispose();
    // }

}
