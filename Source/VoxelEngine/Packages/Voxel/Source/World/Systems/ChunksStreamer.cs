using System.Numerics;
using VoxelEngine.Core;
using VoxelEngine.Diagnostics;
using VoxelEngine.Input;

namespace VoxelEngine.Packages.Voxel;

public sealed class ChunksStreamer : IDisposable, IUpdatable, ITickable
{
    public bool IsChunkStreamingEnabled { get; private set; } = true;

    public int SimulationDistance { get; set; } = 3;
    public int ViewDistance { get; set; } = 5;
    public float ChunksUnloadDistance => ViewDistance + 2;
    public Int3 _lastChunkPos = Int3.MaxValue;

    private readonly List<Int3> _chunksToLoad = new();

    private World _world;
    private VoxelWorld _voxelWorld;
    // private Scene _scene;
    private QueryDescription _query;

    public ChunksStreamer(VoxelWorld ve, World world)
    {
        _voxelWorld = ve;
        _world = world;
        _query = new QueryDescription().WithAll<C_Transform, C_Camera>();
    }

    public void OnUpdate()
    {
        if (InputManager.GetKeyDown(Key.F1))
        {
            IsChunkStreamingEnabled = !IsChunkStreamingEnabled;
            Logger.Debug($"Chunk streming toggled to: {IsChunkStreamingEnabled}");
        }
    }

    public void OnTick()
    {
        TryStartChunksStreaming();
        LoadChunks();
    }

    private void LoadChunks()
    {
        foreach (Int3 pos in _chunksToLoad)
        {
            _voxelWorld.ChunksStorage.GetOrCreateChunk(pos);
        }
    }

    private void TryStartChunksStreaming()
    {
        if (!IsChunkStreamingEnabled) return;

        Vector3 cameraPos = Vector3.Zero;
        bool cameraFound = false;
        _world.Query(in _query, (ref C_Transform transform, ref C_Camera camera) =>
        {
            if (!cameraFound)
            {
                cameraPos = transform.WorldPosition;
                cameraFound = true;
            }
        });
        if (!cameraFound) return;

        Int3 center = ChunksHelper.GetChunkPosition(cameraPos);
        if (_lastChunkPos == center) return;
        _lastChunkPos = center;

        _chunksToLoad.Clear();
        StreamChunks(center);
    }
    private void StreamChunks(Int3 center)
    {
        int viewDistSqr = ViewDistance * ViewDistance;

        for (int x = -ViewDistance; x <= ViewDistance; x++)
        {
            for (int y = -ViewDistance; y <= ViewDistance; y++)
            {
                for (int z = -ViewDistance; z <= ViewDistance; z++)
                {
                    if (x * x + y * y + z * z > viewDistSqr) continue;

                    Int3 chunkPos = new Int3(x * Chunk.SIZE, y * Chunk.SIZE, z * Chunk.SIZE) + center;
                    if (!_voxelWorld.ChunksStorage.ContainsChunk(chunkPos))
                    {
                        _chunksToLoad.Add(chunkPos);
                    }
                }
            }
        }

        float unloadDisSqr = ChunksUnloadDistance * Chunk.SIZE;
        unloadDisSqr *= unloadDisSqr;

        var allChunks = _voxelWorld.ChunksStorage.GetAllChunks();
        for (int i = allChunks.Length - 1; i >= 0; i--)
        {
            Chunk c = allChunks[i];
            Vector3 chunkCenter = new Vector3(c.Position.X + Chunk.SIZE / 2f, c.Position.Y + Chunk.SIZE / 2f, c.Position.Z + Chunk.SIZE / 2f);
            if (Vector3.DistanceSquared((Vector3)center, chunkCenter) > unloadDisSqr)
            {
                _voxelWorld.ChunksStorage.RemoveChunk(c.Position);
            }
        }

        _chunksToLoad.Sort((a, b) =>
        {
            Vector3 centerA = new Vector3(a.X + Chunk.SIZE / 2f, a.Y + Chunk.SIZE / 2f, a.Z + Chunk.SIZE / 2f);
            Vector3 centerB = new Vector3(b.X + Chunk.SIZE / 2f, b.Y + Chunk.SIZE / 2f, b.Z + Chunk.SIZE / 2f);
            return Vector3.DistanceSquared((Vector3)center, centerA).CompareTo(Vector3.DistanceSquared((Vector3)center, centerB));
        });
    }

    public void Dispose()
    {
    }
}
