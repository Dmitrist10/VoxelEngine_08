using System.Numerics;
using VoxelEngine.Core;

namespace VoxelEngine.Packages.Voxel;

public sealed class ChunkMesher
{
    [ThreadStatic]
    private static List<VoxelVertex>? _verticesBuffer;

    [ThreadStatic]
    private static List<uint>? _indicesBuffer;

    [MethodImpl(AggressiveOptimization)]
    public static VoxelMeshData? CreateMesh(Chunk chunk)
    {
        _verticesBuffer ??= new List<VoxelVertex>(8192);
        _indicesBuffer ??= new List<uint>(12288);

        _verticesBuffer.Clear();
        _indicesBuffer.Clear();

        int CHUNK_SIZE = Chunk.SIZE;

        for (int d = 0; d < 3; d++)
        {
            int i, j, k, l, w, h;
            int u = (d + 1) % 3;
            int v = (d + 2) % 3;
            int[] x = new int[3];
            int[] q = new int[3];
            uint[] mask = new uint[CHUNK_SIZE * CHUNK_SIZE];

            q[d] = 1;

            for (x[d] = -1; x[d] < CHUNK_SIZE;)
            {
                int n = 0;
                for (x[v] = 0; x[v] < CHUNK_SIZE; x[v]++)
                {
                    for (x[u] = 0; x[u] < CHUNK_SIZE; x[u]++)
                    {
                        uint block1 = 0;
                        uint block2 = 0;

                        if (x[d] >= 0)
                            block1 = chunk.GetBlockUnsafe(x[0], x[1], x[2]);
                        else
                            block1 = chunk.GetBlock(x[0], x[1], x[2]);

                        if (x[d] < CHUNK_SIZE - 1)
                            block2 = chunk.GetBlockUnsafe(x[0] + q[0], x[1] + q[1], x[2] + q[2]);
                        else
                            block2 = chunk.GetBlock(x[0] + q[0], x[1] + q[1], x[2] + q[2]);

                        bool b1Solid = block1 != 0;
                        bool b2Solid = block2 != 0;

                        if (b1Solid && !b2Solid)
                        {
                            mask[n++] = block1; // Forward face
                        }
                        else if (!b1Solid && b2Solid)
                        {
                            mask[n++] = block2 | 0x80000000; // Backward face
                        }
                        else
                        {
                            mask[n++] = 0;
                        }
                    }
                }

                x[d]++;
                n = 0;

                for (j = 0; j < CHUNK_SIZE; j++)
                {
                    for (i = 0; i < CHUNK_SIZE;)
                    {
                        uint maskVal = mask[n];
                        if (maskVal != 0)
                        {
                            for (w = 1; i + w < CHUNK_SIZE && mask[n + w] == maskVal; w++) { }

                            bool done = false;
                            for (h = 1; j + h < CHUNK_SIZE; h++)
                            {
                                for (k = 0; k < w; k++)
                                {
                                    if (mask[n + k + h * CHUNK_SIZE] != maskVal)
                                    {
                                        done = true;
                                        break;
                                    }
                                }
                                if (done) break;
                            }

                            bool isBackFace = (maskVal & 0x80000000) != 0;
                            uint blockId = maskVal & 0x7FFFFFFF;

                            x[u] = i;
                            x[v] = j;

                            int[] du = new int[3]; du[u] = w;
                            int[] dv = new int[3]; dv[v] = h;

                            Vector3 basePos = new Vector3(x[0], x[1], x[2]);
                            Vector3 duVec = new Vector3(du[0], du[1], du[2]);
                            Vector3 dvVec = new Vector3(dv[0], dv[1], dv[2]);

                            Face face = DetermineFace(d, isBackFace);
                            AddGreedyQuad(basePos, duVec, dvVec, blockId, face, w, h, _verticesBuffer, _indicesBuffer);

                            for (l = 0; l < h; l++)
                            {
                                for (k = 0; k < w; k++)
                                {
                                    mask[n + k + l * CHUNK_SIZE] = 0;
                                }
                            }

                            i += w;
                            n += w;
                        }
                        else
                        {
                            i++;
                            n++;
                        }
                    }
                }
            }
        }

        if (_verticesBuffer.Count == 0 || _indicesBuffer.Count == 0)
            return null;

        return new VoxelMeshData(_verticesBuffer.ToArray(), _indicesBuffer.ToArray());
    }

    [MethodImpl(AggressiveOptimization)]
    private static Face DetermineFace(int d, bool isBackFace)
    {
        if (d == 0) return isBackFace ? Face.Left : Face.Right;      // X Axis
        if (d == 1) return isBackFace ? Face.Bottom : Face.Top;      // Y Axis
        return isBackFace ? Face.Back : Face.Front;                  // Z Axis
    }

    [MethodImpl(AggressiveOptimization)]
    private static void AddGreedyQuad(Vector3 basePos, Vector3 du, Vector3 dv, uint blockId, Face face, int w, int h, List<VoxelVertex> vertices, List<uint> indices)
    {
        uint baseIdx = (uint)vertices.Count;
        int textureLayerIndex = GetTextureLayerForBlock(blockId, face);

        Vector3 normal = Vector3.Zero;
        switch (face)
        {
            case Face.Front: normal = new Vector3(0, 0, 1); break;
            case Face.Back: normal = new Vector3(0, 0, -1); break;
            case Face.Left: normal = new Vector3(-1, 0, 0); break;
            case Face.Right: normal = new Vector3(1, 0, 0); break;
            case Face.Top: normal = new Vector3(0, 1, 0); break;
            case Face.Bottom: normal = new Vector3(0, -1, 0); break;
        }

        Vector3 p0 = basePos;
        Vector3 p1 = basePos + du;
        Vector3 p2 = basePos + du + dv;
        Vector3 p3 = basePos + dv;

        // Determine UV wrapping properly based on W and H
        // For Greedy, UVs map exactly the w and h scaling block counts.
        // Usually, U correlates to w, V correlates to h.
        // Depending on standard axis winding, we assign the corners correctly.
        bool windingFlip = false;
        if (face == Face.Left || face == Face.Bottom || face == Face.Back)
        {
            windingFlip = true;
        }

        if (!windingFlip)
        {
            vertices.Add(new VoxelVertex(p0, new Vector2(0, 0), textureLayerIndex, (int)face));
            vertices.Add(new VoxelVertex(p1, new Vector2(w, 0), textureLayerIndex, (int)face));
            vertices.Add(new VoxelVertex(p2, new Vector2(w, h), textureLayerIndex, (int)face));
            vertices.Add(new VoxelVertex(p3, new Vector2(0, h), textureLayerIndex, (int)face));

            indices.Add(baseIdx + 0);
            indices.Add(baseIdx + 1);
            indices.Add(baseIdx + 2);
            indices.Add(baseIdx + 0);
            indices.Add(baseIdx + 2);
            indices.Add(baseIdx + 3);
        }
        else
        {
            vertices.Add(new VoxelVertex(p0, new Vector2(0, 0), textureLayerIndex, (int)face));
            vertices.Add(new VoxelVertex(p1, new Vector2(w, 0), textureLayerIndex, (int)face));
            vertices.Add(new VoxelVertex(p2, new Vector2(w, h), textureLayerIndex, (int)face));
            vertices.Add(new VoxelVertex(p3, new Vector2(0, h), textureLayerIndex, (int)face));

            // Flipped indices
            indices.Add(baseIdx + 0);
            indices.Add(baseIdx + 2);
            indices.Add(baseIdx + 1);
            indices.Add(baseIdx + 0);
            indices.Add(baseIdx + 3);
            indices.Add(baseIdx + 2);
        }
    }

    [MethodImpl(AggressiveInlining)]
    private static int GetTextureLayerForBlock(uint blockId, Face face) => Math.Max(0, (int)blockId - 1);
}