using Assimp;
using System.Numerics;

namespace VoxelEngine.Core.Assets;

public class MeshLoader : IAssetLoader
{
    public IAssetData Load(string path)
    {
        var context = new AssimpContext();
        // PostProcessSteps.Triangulate ensures all faces are triangles
        // PostProcessSteps.GenerateNormals ensures we have lighting data
        Assimp.Scene scene = context.ImportFile(path, PostProcessSteps.Triangulate | PostProcessSteps.GenerateNormals);

        if (scene == null || scene.Meshes.Count == 0)
            throw new Exception("Failed to load mesh!");

        // For simplicity, we grab the first mesh in the file
        var assimpMesh = scene.Meshes[0];

        var vertices = new List<STDVertex>();
        var indices = new List<uint>();

        // 1. Extract Vertices
        for (int i = 0; i < assimpMesh.VertexCount; i++)
        {
            var pos = assimpMesh.Vertices[i];
            var norm = assimpMesh.HasNormals ? assimpMesh.Normals[i] : new Vector3D(0, 1, 0);
            Vector2 texC = Vector2.Zero;

            if (assimpMesh.HasTextureCoords(0))
            {
                var coord = assimpMesh.TextureCoordinateChannels[0][i];
                texC = new Vector2(coord.X, coord.Y);
            }

            vertices.Add(new STDVertex
            {
                Position = new Vector3(pos.X, pos.Y, pos.Z),
                Normal = new Vector3(norm.X, norm.Y, norm.Z),
                UV = texC
            });
        }

        // 2. Extract Indices
        foreach (var face in assimpMesh.Faces)
        {
            if (face.IndexCount == 3)
            {
                indices.Add((uint)face.Indices[0]);
                indices.Add((uint)face.Indices[1]);
                indices.Add((uint)face.Indices[2]);
            }
        }

        return new STDMeshData(vertices.ToArray(), indices.ToArray());
    }
}