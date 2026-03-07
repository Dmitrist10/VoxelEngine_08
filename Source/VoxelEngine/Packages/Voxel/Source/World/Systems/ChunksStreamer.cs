using System.Numerics;
using VoxelEngine.Core;
using VoxelEngine.Diagnostics;

namespace VoxelEngine.Packages.Voxel;

public sealed class ChunksStreamer : IDisposable
{
    private readonly ChunksStorage _storage;
    private Int3 _lastBaseChunkPos;
    private readonly List<Int3> _chunksToLoad = new();

    public bool IsChunkStreamingEnabled { get; set; } = true;
    public int ViewDistance { get; set; } = 8;
    private readonly World _world;

    private QueryDescription _query;

    public ChunksStreamer(ChunksStorage storage, World world)
    {
        _storage = storage;
        _world = world;
        _query = new QueryDescription().WithAll<C_Transform, C_Camera>();
    }

    public void OnUpdate()
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

        Int3 baseChunkPos = new Int3(
            (int)Math.Floor(cameraPos.X / Chunk.SIZE) * Chunk.SIZE,
            (int)Math.Floor(cameraPos.Y / Chunk.SIZE) * Chunk.SIZE,
            (int)Math.Floor(cameraPos.Z / Chunk.SIZE) * Chunk.SIZE
        );

        if (_lastBaseChunkPos == baseChunkPos && _chunksToLoad.Count == 0) return;
        _lastBaseChunkPos = baseChunkPos;
        _chunksToLoad.Clear();

        int viewDistSqr = ViewDistance * ViewDistance;
        for (int x = -ViewDistance; x <= ViewDistance; x++)
        {
            for (int y = -ViewDistance; y <= ViewDistance; y++)
            {
                for (int z = -ViewDistance; z <= ViewDistance; z++)
                {
                    if (x * x + y * y + z * z > viewDistSqr) continue;

                    Int3 chunkPos = new Int3(x * Chunk.SIZE, y * Chunk.SIZE, z * Chunk.SIZE) + baseChunkPos;
                    if (!_storage.ContainsChunk(chunkPos))
                    {
                        Chunk chunk = new Chunk(chunkPos);
                        _storage.Add(chunk);
                    }
                        // _chunksToLoad.Add(chunkPos);
                }
            }
        }

        float unloadDisSqr = ((float)ViewDistance + 2f) * (float)Chunk.SIZE;
        unloadDisSqr *= unloadDisSqr;

        var allChunks = _storage.GetAllChunks();
        for (int i = allChunks.Length - 1; i >= 0; i--)
        {
            Chunk c = allChunks[i];
            Vector3 chunkCenter = new Vector3(c.Position.X + Chunk.SIZE / 2f, c.Position.Y + Chunk.SIZE / 2f, c.Position.Z + Chunk.SIZE / 2f);
            if (Vector3.DistanceSquared(cameraPos, chunkCenter) > unloadDisSqr)
            {
                _storage.RemoveChunk(c.Position);
            }
        }

        _chunksToLoad.Sort((a, b) =>
        {
            Vector3 centerA = new Vector3(a.X + Chunk.SIZE / 2f, a.Y + Chunk.SIZE / 2f, a.Z + Chunk.SIZE / 2f);
            Vector3 centerB = new Vector3(b.X + Chunk.SIZE / 2f, b.Y + Chunk.SIZE / 2f, b.Z + Chunk.SIZE / 2f);
            return Vector3.DistanceSquared(cameraPos, centerA).CompareTo(Vector3.DistanceSquared(cameraPos, centerB));
        });

        // Load a few chunks per frame for now to avoid freezing
        int chunksToInstantiate = Math.Min(4, _chunksToLoad.Count);
        for (int i = 0; i < chunksToInstantiate; i++)
        {
            var pos = _chunksToLoad[0];
            _chunksToLoad.RemoveAt(0);

            Chunk newChunk = _storage.GetChunk(pos);

            // Temporary simple terrain generation (floor)
            if (pos.Y == 0)
            {
                for (int tx = 0; tx < Chunk.SIZE; tx++)
                    for (int tz = 0; tz < Chunk.SIZE; tz++)
                        newChunk.SetBlock(tx, 0, tz, 1);
            }
        }
    }

    // public void ProcessLoadQueue(ChunkStorage storage, ChunkPhysics physics, B_VoxelPlayer? player, int viewDist, int simDist)
    // {
    //     int uploadedThisFrame = 0;
    //     while (uploadedThisFrame < MAX_CHUNKS_UPLOADED_PER_TICK && _generatedMeshesQueue.TryDequeue(out var chunk))
    //     {
    //         _generatingChunks.Remove(chunk.Position);

    //         float unloadDis = ((float)viewDist + 2f) * (float)Chunk.CHUNK_SIZE;
    //         if (player != null && Vector3.Distance(player.Owner.Position, (Vector3)chunk.Position) > unloadDis)
    //         {
    //             chunk.Dispose();
    //             continue;
    //         }

    //         if (chunk.Mesh != null)
    //         {
    //             chunk.UploadMesh(_factory);
    //         }

    //         storage.AddChunk(chunk);
    //         physics.CheckChunk(chunk, player, simDist);

    //         chunk.FreeCPUMeshData();
    //         uploadedThisFrame++;
    //     }

    //     int runningTasks = _generatingChunks.Count;
    //     int maxWorkers = Math.Max(2, Environment.ProcessorCount - 2);

    //     while (_chunksToLoad.Count > 0 && runningTasks < maxWorkers)
    //     {
    //         var pos = _chunksToLoad[0];
    //         _chunksToLoad.RemoveAt(0);

    //         if (_generatingChunks.Contains(pos) || storage.ContainsChunk(pos))
    //             continue;

    //         _generatingChunks.Add(pos);
    //         runningTasks++;

    //         Task.Run(() =>
    //         {
    //             if (_cts.IsCancellationRequested) return;

    //             var blocks = WorldGenerator.CreateChunk(pos);
    //             Chunk newChunk = new Chunk(blocks, pos);
    //             newChunk.GenerateMesh();

    //             _generatedMeshesQueue.Enqueue(newChunk);
    //         }, _cts.Token);
    //     }
    // }

    public void Dispose()
    {
        _chunksToLoad.Clear();
    }
}
