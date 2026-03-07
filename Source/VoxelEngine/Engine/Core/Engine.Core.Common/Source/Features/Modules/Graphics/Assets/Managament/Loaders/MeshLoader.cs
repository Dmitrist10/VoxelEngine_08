using Assimp;
using System.Numerics;

namespace VoxelEngine.Core.Assets;

public class MeshLoader : IAssetLoader<STDMeshData>
{

    public STDMeshData Load(string absolutePath)
    {
        var context = new AssimpContext();
        Assimp.Scene scene = context.ImportFile(absolutePath, PostProcessSteps.Triangulate | PostProcessSteps.GenerateNormals);

        if (scene == null || scene.Meshes.Count == 0)
            throw new Exception("Failed to load mesh!");

        var assimpMesh = scene.Meshes[0]; // TODO: Support multiple meshes
        var assimpVertices = new List<STDVertex>();
        var assimpIndices = new List<uint>();

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

            assimpVertices.Add(new STDVertex(
                new Vector3(pos.X, pos.Y, pos.Z),
                new Vector3(norm.X, norm.Y, norm.Z),
                texC
            ));
        }

        foreach (var face in assimpMesh.Faces)
        {
            if (face.IndexCount == 3)
            {
                assimpIndices.Add((uint)face.Indices[0]);
                assimpIndices.Add((uint)face.Indices[1]);
                assimpIndices.Add((uint)face.Indices[2]);
            }
        }

        return new STDMeshData(assimpVertices.ToArray(), assimpIndices.ToArray());
    }

}

// public class MeshLoader : IAssetLoader<STDMeshData>
// {

//     public STDMeshData Load(Stream stream, AssetId id, string absolutePath)
//     {
//         // Check if stream is our fast VE_MESH binary format
//         if (absolutePath.EndsWith(".ve_mesh", StringComparison.OrdinalIgnoreCase))
//         {
//             using var reader = new BinaryReader(stream, Encoding.UTF8, true);
//             string magic = reader.ReadString();
//             if (magic != "VE_MESH") throw new Exception("Invalid VE_MESH format!");

//             uint vCount = reader.ReadUInt32();
//             uint iCount = reader.ReadUInt32();

//             STDVertex[] vertices = new STDVertex[vCount];
//             for (uint i = 0; i < vCount; i++)
//             {
//                 vertices[i] = new STDVertex(
//                     new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
//                     new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
//                     new Vector2(reader.ReadSingle(), reader.ReadSingle())
//                 );
//             }

//             uint[] indices = new uint[iCount];
//             for (uint i = 0; i < iCount; i++)
//             {
//                 indices[i] = reader.ReadUInt32();
//             }

//             return new STDMeshData(vertices, indices);
//         }

//         // Fast fallback: if we have a real file path and Assimp supports it, use Assimp directly.
//         // The IVirtualFileProvider provides `absolutePath` when it maps to physical disk.
//         var context = new AssimpContext();
//         Assimp.Scene scene = context.ImportFile(absolutePath, PostProcessSteps.Triangulate | PostProcessSteps.GenerateNormals);

//         if (scene == null || scene.Meshes.Count == 0)
//             throw new Exception("Failed to load mesh!");

//         var assimpMesh = scene.Meshes[0];
//         var assimpVertices = new List<STDVertex>();
//         var assimpIndices = new List<uint>();

//         for (int i = 0; i < assimpMesh.VertexCount; i++)
//         {
//             var pos = assimpMesh.Vertices[i];
//             var norm = assimpMesh.HasNormals ? assimpMesh.Normals[i] : new Vector3D(0, 1, 0);
//             Vector2 texC = Vector2.Zero;

//             if (assimpMesh.HasTextureCoords(0))
//             {
//                 var coord = assimpMesh.TextureCoordinateChannels[0][i];
//                 texC = new Vector2(coord.X, coord.Y);
//             }

//             assimpVertices.Add(new STDVertex(
//                 new Vector3(pos.X, pos.Y, pos.Z),
//                 new Vector3(norm.X, norm.Y, norm.Z),
//                 texC
//             ));
//         }

//         foreach (var face in assimpMesh.Faces)
//         {
//             if (face.IndexCount == 3)
//             {
//                 assimpIndices.Add((uint)face.Indices[0]);
//                 assimpIndices.Add((uint)face.Indices[1]);
//                 assimpIndices.Add((uint)face.Indices[2]);
//             }
//         }

//         return new STDMeshData(assimpVertices.ToArray(), assimpIndices.ToArray());
//     }
// }