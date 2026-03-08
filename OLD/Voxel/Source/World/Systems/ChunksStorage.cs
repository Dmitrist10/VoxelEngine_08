using System.Runtime.InteropServices;
using VoxelEngine.Core;

namespace VoxelEngine.Packages.Voxel;

public sealed class ChunksStorage
{
    private readonly Dictionary<Int3, Chunk> _chunks = new();
    private readonly List<Chunk> _allChunks = new();

    private WorldGenerator _worldGenerator;

    public ChunksStorage()
    {
        _worldGenerator = new WorldGenerator();
    }

    public void Add(Chunk c)
    {
        _allChunks.Add(c);
        _chunks.Add(c.Position, c);
    }

    public Chunk GetOrCreateChunk(Int3 pos)
    {
        if (_chunks.TryGetValue(pos, out var chunk))
            return chunk;

        chunk = new Chunk(pos, WorldGenerator.CreateChunk(pos));
        _chunks.Add(pos, chunk);
        _allChunks.Add(chunk);
        return chunk;
    }
    public Chunk GetChunk(Int3 pos)
    {
        if (_chunks.TryGetValue(pos, out var chunk))
            return chunk;

        chunk = new Chunk(pos, WorldGenerator.CreateChunk(pos));
        _chunks.Add(pos, chunk);
        _allChunks.Add(chunk);
        return chunk;
    }

    public void RemoveChunk(Int3 pos)
    {
        if (_chunks.TryGetValue(pos, out var chunk))
        {
            chunk.Dispose();
            _chunks.Remove(pos);

            int index = _allChunks.IndexOf(chunk);
            if (index >= 0)
            {
                int lastIndex = _allChunks.Count - 1;
                _allChunks[index] = _allChunks[lastIndex];
                _allChunks.RemoveAt(lastIndex);
            }
        }
    }


    internal ReadOnlySpan<Chunk> GetAllChunks() => CollectionsMarshal.AsSpan(_allChunks);

    [MethodImpl(AggressiveInlining)]
    public bool ContainsChunk(Int3 pos) => _chunks.ContainsKey(pos);

    public void Clear()
    {
        foreach (var chunk in _allChunks)
            chunk.Dispose();

        _chunks.Clear();
        _allChunks.Clear();
    }

    public void Dispose()
    {
        Clear();
    }
}
