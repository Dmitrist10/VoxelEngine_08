using System;
using System.Collections.Generic;
using VoxelEngine.Core;

namespace VoxelEngine.Packages.Voxel;

public sealed class WorldGenerator
{
    private static SimplexNoise _noise = new SimplexNoise(seed: 137);

    public static uint[] CreateChunk(Int3 chunkPos)
    {
        uint[] blocks = new uint[Chunk.SIZE * Chunk.SIZE * Chunk.SIZE];

        int topY = chunkPos.Y + Chunk.SIZE - 1;
        if (topY < 20)
        {
            Array.Fill(blocks, 5u);
            return blocks;
        }

        for (int x = 0; x < Chunk.SIZE; x++)
        {
            for (int z = 0; z < Chunk.SIZE; z++)
            {
                int worldX = chunkPos.X + x;
                int worldZ = chunkPos.Z + z;

                // 1. Calculate traits via 2D smooth noise
                float temperature = _noise.FBM(worldX, worldZ, octaves: 2, scale: 0.002f);
                float humidity = _noise.FBM(worldX + 10000, worldZ + 10000, octaves: 2, scale: 0.002f);

                // 2. Select Biome from Data-Driven Registry
                Biome currentBiome = BiomeRegistry.GetBiome(temperature, humidity);

                // 3. Terrain Shape (Continentalness & Erosion)
                // Using data-driven values instead of hardcoded numbers
                float continentalness = _noise.FBM(worldX + 5000, worldZ + 5000, octaves: 4, scale: 0.0005f);
                float erosion = _noise.FBM(worldX + 20000, worldZ + 20000, octaves: 4, scale: 0.001f);

                int baseHeight = currentBiome.BaseHeight + (int)(continentalness * currentBiome.ContinentalnessScale);

                float normalizedErosion = Math.Max(0, erosion + 0.3f);
                float mountainSpike = (float)Math.Pow(normalizedErosion, currentBiome.ErosionSpikePower);
                int heightVariation = 2 + (int)(mountainSpike * currentBiome.MaxHeightVariation);

                float detailNoise = _noise.FBM(worldX, worldZ, octaves: 6, scale: 0.008f);
                float normalizedDetail = (detailNoise + 1f) / 2f;
                int surfaceHeight = baseHeight + (int)(normalizedDetail * heightVariation);

                for (int y = 0; y < Chunk.SIZE; y++)
                {
                    int worldY = chunkPos.Y + y;
                    uint block = 0;

                    if (worldY == surfaceHeight)
                    {
                        block = currentBiome.SurfaceBlock;
                    }
                    else if (worldY < surfaceHeight && worldY >= surfaceHeight - currentBiome.SurfaceDepth)
                    {
                        block = currentBiome.SubSurfaceBlock;
                    }
                    else if (worldY < surfaceHeight - currentBiome.SurfaceDepth)
                    {
                        block = 5; // Deep Stone
                    }

                    blocks[Chunk.GetBlockIndex(x, y, z)] = block;
                }
            }
        }
        return blocks;
    }
}