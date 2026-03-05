using System.Numerics;
using VoxelEngine.Core;

namespace VoxelEngine.Packages.Voxel;

public static class ChunksHelper
{
    public static Int3 GetChunkPosition(Vector3 position)
    {
        return new Int3(
            (int)System.MathF.Floor(position.X / Chunk.SIZE) * Chunk.SIZE,
            (int)System.MathF.Floor(position.Y / Chunk.SIZE) * Chunk.SIZE,
            (int)System.MathF.Floor(position.Z / Chunk.SIZE) * Chunk.SIZE
        );
    }
    public static Int3 GetChunkPositionGrid(Vector3 position)
    {
        return new Int3(
            (int)System.MathF.Floor(position.X / Chunk.SIZE),
            (int)System.MathF.Floor(position.Y / Chunk.SIZE),
            (int)System.MathF.Floor(position.Z / Chunk.SIZE)
        );
    }
    public static Int3 GetBlockPosition(Vector3 position)
    {
        float x = System.MathF.Floor(position.X) % Chunk.SIZE;
        float y = System.MathF.Floor(position.Y) % Chunk.SIZE;
        float z = System.MathF.Floor(position.Z) % Chunk.SIZE;

        if (x < 0) x += Chunk.SIZE;
        if (y < 0) y += Chunk.SIZE;
        if (z < 0) z += Chunk.SIZE;

        return new Int3((int)x, (int)y, (int)z);
    }


}