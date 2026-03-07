using System.Numerics;
using System.Text;
using VoxelEngine.Core;

namespace VoxelEngine.IO;

internal class BuiltInFilesProvider : IFilesProvider
{
    private readonly string _alias;
    private Dictionary<string, Func<Stream>> _generators = new Dictionary<string, Func<Stream>>(StringComparer.OrdinalIgnoreCase);
    
    public BuiltInFilesProvider(string alias)
    {
        _alias = alias;

        _generators["Primitives/Triangle.ve"] = () => SerializeMesh(GetTriangle());
        _generators["Primitives/Quad.ve"] = () => SerializeMesh(GetQuad());
        _generators["Primitives/Cube.ve"] = () => SerializeMesh(GetCube());
        _generators["Primitives/Sphere.ve"] = () => SerializeMesh(GetSphere());
        _generators["Primitives/Pyramid.ve"] = () => SerializeMesh(GetPyramid());
        _generators["Primitives/Cylinder.ve"] = () => SerializeMesh(GetCylinder());
        _generators["Primitives/Circle.ve"] = () => SerializeMesh(GetCircle());

        // // Setup built-in shaders
        // _generators["Shaders/UberShader.vert"] = () => GenerateTextStream(UBER_SHADER_VERT);
        // _generators["Shaders/UberShader.frag"] = () => GenerateTextStream(UBER_SHADER_FRAG);
    }

    private Stream GenerateTextStream(string text)
    {
        var ms = new MemoryStream();
        var writer = new StreamWriter(ms, Encoding.UTF8, 1024, true);
        writer.Write(text);
        writer.Flush();
        ms.Position = 0;
        return ms;
    }

    private Stream SerializeMesh((STDVertex[] vertices, uint[] indices) mesh)
    {
        var ms = new MemoryStream();
        using var writer = new BinaryWriter(ms, Encoding.UTF8, true);

        // Magic string
        writer.Write("VE_MESH");

        writer.Write((uint)mesh.vertices.Length);
        writer.Write((uint)mesh.indices.Length);

        foreach (var v in mesh.vertices)
        {
            writer.Write(v.Position.X); writer.Write(v.Position.Y); writer.Write(v.Position.Z); // Position
            writer.Write(v.Normal.X); writer.Write(v.Normal.Y); writer.Write(v.Normal.Z); // Normal
            writer.Write(v.UV.X); writer.Write(v.UV.Y); // UV
        }

        // Indecies
        foreach (var i in mesh.indices)
        {
            writer.Write(i);
        }

        ms.Position = 0;
        return ms;
    }

    public Stream OpenRead(string path)
    {
        // Normalize path separators
        path = path.Replace('\\', '/');

        if (_generators.TryGetValue(path, out Func<Stream>? generator))
        {
            return generator();
        }

        throw new FileNotFoundException($"Built-in asset not found: {path}");
    }


    private (STDVertex[], uint[]) GetTriangle()
    {
        STDVertex[] vertices = {
            new STDVertex(new Vector3(-0.5f, -0.5f, 0), new Vector3(0, 0, 1), new Vector2(0, 0)),
            new STDVertex(new Vector3( 0.5f, -0.5f, 0), new Vector3(0, 0, 1), new Vector2(1, 0)),
            new STDVertex(new Vector3( 0.0f,  0.5f, 0), new Vector3(0, 0, 1), new Vector2(0.5f, 1)),
        };
        uint[] indices = { 0, 1, 2 };
        return (vertices, indices);
    }

    private (STDVertex[], uint[]) GetQuad()
    {
        STDVertex[] vertices = {
            new STDVertex(new Vector3(-0.5f, -0.5f, 0), new Vector3(0, 0, 1), new Vector2(0, 0)),
            new STDVertex(new Vector3( 0.5f, -0.5f, 0), new Vector3(0, 0, 1), new Vector2(1, 0)),
            new STDVertex(new Vector3( 0.5f,  0.5f, 0), new Vector3(0, 0, 1), new Vector2(1, 1)),
            new STDVertex(new Vector3(-0.5f,  0.5f, 0), new Vector3(0, 0, 1), new Vector2(0, 1)),
        };
        uint[] indices = { 0, 1, 2, 2, 3, 0 };
        return (vertices, indices);
    }

    private (STDVertex[], uint[]) GetCube()
    {
        STDVertex[] vertices = {
            // Front face  (+Z)
            new STDVertex(new Vector3(-0.5f, -0.5f,  0.5f), new Vector3( 0,  0,  1), new Vector2(0, 0)),
            new STDVertex(new Vector3( 0.5f, -0.5f,  0.5f), new Vector3( 0,  0,  1), new Vector2(1, 0)),
            new STDVertex(new Vector3( 0.5f,  0.5f,  0.5f), new Vector3( 0,  0,  1), new Vector2(1, 1)),
            new STDVertex(new Vector3(-0.5f,  0.5f,  0.5f), new Vector3( 0,  0,  1), new Vector2(0, 1)),

            // Back face  (-Z)
            new STDVertex(new Vector3( 0.5f, -0.5f, -0.5f), new Vector3( 0,  0, -1), new Vector2(0, 0)),
            new STDVertex(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3( 0,  0, -1), new Vector2(1, 0)),
            new STDVertex(new Vector3(-0.5f,  0.5f, -0.5f), new Vector3( 0,  0, -1), new Vector2(1, 1)),
            new STDVertex(new Vector3( 0.5f,  0.5f, -0.5f), new Vector3( 0,  0, -1), new Vector2(0, 1)),

            // Left face  (-X)
            new STDVertex(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(-1,  0,  0), new Vector2(0, 0)),
            new STDVertex(new Vector3(-0.5f, -0.5f,  0.5f), new Vector3(-1,  0,  0), new Vector2(1, 0)),
            new STDVertex(new Vector3(-0.5f,  0.5f,  0.5f), new Vector3(-1,  0,  0), new Vector2(1, 1)),
            new STDVertex(new Vector3(-0.5f,  0.5f, -0.5f), new Vector3(-1,  0,  0), new Vector2(0, 1)),

            // Right face  (+X)
            new STDVertex(new Vector3( 0.5f, -0.5f,  0.5f), new Vector3( 1,  0,  0), new Vector2(0, 0)),
            new STDVertex(new Vector3( 0.5f, -0.5f, -0.5f), new Vector3( 1,  0,  0), new Vector2(1, 0)),
            new STDVertex(new Vector3( 0.5f,  0.5f, -0.5f), new Vector3( 1,  0,  0), new Vector2(1, 1)),
            new STDVertex(new Vector3( 0.5f,  0.5f,  0.5f), new Vector3( 1,  0,  0), new Vector2(0, 1)),

            // Top face  (+Y)
            new STDVertex(new Vector3(-0.5f,  0.5f,  0.5f), new Vector3( 0,  1,  0), new Vector2(0, 0)),
            new STDVertex(new Vector3( 0.5f,  0.5f,  0.5f), new Vector3( 0,  1,  0), new Vector2(1, 0)),
            new STDVertex(new Vector3( 0.5f,  0.5f, -0.5f), new Vector3( 0,  1,  0), new Vector2(1, 1)),
            new STDVertex(new Vector3(-0.5f,  0.5f, -0.5f), new Vector3( 0,  1,  0), new Vector2(0, 1)),

            // Bottom face  (-Y)
            new STDVertex(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3( 0, -1,  0), new Vector2(0, 0)),
            new STDVertex(new Vector3( 0.5f, -0.5f, -0.5f), new Vector3( 0, -1,  0), new Vector2(1, 0)),
            new STDVertex(new Vector3( 0.5f, -0.5f,  0.5f), new Vector3( 0, -1,  0), new Vector2(1, 1)),
            new STDVertex(new Vector3(-0.5f, -0.5f,  0.5f), new Vector3( 0, -1,  0), new Vector2(0, 1)),
        };

        uint[] indices = {
             0,  1,  2,  2,  3,  0,  // Front
             4,  5,  6,  6,  7,  4,  // Back
             8,  9, 10, 10, 11,  8,  // Left
            12, 13, 14, 14, 15, 12,  // Right
            16, 17, 18, 18, 19, 16,  // Top
            20, 21, 22, 22, 23, 20,  // Bottom
        };
        return (vertices, indices);
    }

    private (STDVertex[], uint[]) GetSphere()
    {
        int sectors = 24;
        int stacks = 24;
        float radius = 0.5f;

        List<STDVertex> vertices = new List<STDVertex>();
        List<uint> indices = new List<uint>();

        float sectorStep = 2f * System.MathF.PI / sectors;
        float stackStep = System.MathF.PI / stacks;

        for (int i = 0; i <= stacks; ++i)
        {
            float stackAngle = System.MathF.PI / 2 - i * stackStep;
            float xy = radius * System.MathF.Cos(stackAngle);
            float z = radius * System.MathF.Sin(stackAngle);

            for (int j = 0; j <= sectors; ++j)
            {
                float sectorAngle = j * sectorStep;
                float x = xy * System.MathF.Cos(sectorAngle);
                float y = xy * System.MathF.Sin(sectorAngle);

                Vector3 position = new Vector3(x, y, z);
                Vector3 normal = Vector3.Normalize(position);
                Vector2 uv = new Vector2((float)j / sectors, (float)i / stacks);

                vertices.Add(new STDVertex(position, normal, uv));
            }
        }

        for (int i = 0; i < stacks; ++i)
        {
            uint k1 = (uint)(i * (sectors + 1));
            uint k2 = (uint)(k1 + sectors + 1);

            for (int j = 0; j < sectors; ++j, ++k1, ++k2)
            {
                if (i != 0)
                {
                    indices.Add(k1); indices.Add(k2); indices.Add(k1 + 1);
                }
                if (i != (stacks - 1))
                {
                    indices.Add(k1 + 1); indices.Add(k2); indices.Add(k2 + 1);
                }
            }
        }
        return (vertices.ToArray(), indices.ToArray());
    }

    private (STDVertex[], uint[]) GetCylinder()
    {
        int segments = 24;
        float radius = 0.5f;
        float height = 1.0f;
        float halfHeight = height / 2f;

        List<STDVertex> vertices = new List<STDVertex>();
        List<uint> indices = new List<uint>();

        // Sides
        for (int i = 0; i <= segments; i++)
        {
            float angle = (float)i / segments * System.MathF.PI * 2f;
            float x = System.MathF.Cos(angle) * radius;
            float z = System.MathF.Sin(angle) * radius;
            Vector3 normal = new Vector3(System.MathF.Cos(angle), 0, System.MathF.Sin(angle));
            vertices.Add(new STDVertex(new Vector3(x, -halfHeight, z), normal, new Vector2((float)i / segments, 0)));
            vertices.Add(new STDVertex(new Vector3(x, halfHeight, z), normal, new Vector2((float)i / segments, 1)));
        }

        for (int i = 0; i < segments; i++)
        {
            uint b = (uint)(i * 2);
            indices.Add(b); indices.Add(b + 1); indices.Add(b + 2);
            indices.Add(b + 2); indices.Add(b + 1); indices.Add(b + 3);
        }

        // Bottom cap
        uint bottomCenter = (uint)vertices.Count;
        vertices.Add(new STDVertex(new Vector3(0, -halfHeight, 0), -Vector3.UnitY, new Vector2(0.5f, 0.5f)));
        for (int i = 0; i <= segments; i++)
        {
            float angle = (float)i / segments * System.MathF.PI * 2f;
            float x = System.MathF.Cos(angle) * radius;
            float z = System.MathF.Sin(angle) * radius;
            vertices.Add(new STDVertex(new Vector3(x, -halfHeight, z), -Vector3.UnitY, new Vector2(x + 0.5f, z + 0.5f)));
        }
        for (int i = 0; i < segments; i++)
        {
            indices.Add(bottomCenter); indices.Add((uint)(bottomCenter + i + 2)); indices.Add((uint)(bottomCenter + i + 1));
        }

        // Top cap
        uint topCenter = (uint)vertices.Count;
        vertices.Add(new STDVertex(new Vector3(0, halfHeight, 0), Vector3.UnitY, new Vector2(0.5f, 0.5f)));
        for (int i = 0; i <= segments; i++)
        {
            float angle = (float)i / segments * System.MathF.PI * 2f;
            float x = System.MathF.Cos(angle) * radius;
            float z = System.MathF.Sin(angle) * radius;
            vertices.Add(new STDVertex(new Vector3(x, halfHeight, z), Vector3.UnitY, new Vector2(x + 0.5f, z + 0.5f)));
        }
        for (int i = 0; i < segments; i++)
        {
            indices.Add(topCenter); indices.Add((uint)(topCenter + i + 1)); indices.Add((uint)(topCenter + i + 2));
        }
        return (vertices.ToArray(), indices.ToArray());
    }

    private (STDVertex[], uint[]) GetPyramid()
    {
        STDVertex[] vertices = {
            new STDVertex(new Vector3(-0.5f, 0, -0.5f), -Vector3.UnitY, new Vector2(0, 0)),
            new STDVertex(new Vector3( 0.5f, 0, -0.5f), -Vector3.UnitY, new Vector2(1, 0)),
            new STDVertex(new Vector3( 0.5f, 0,  0.5f), -Vector3.UnitY, new Vector2(1, 1)),
            new STDVertex(new Vector3(-0.5f, 0,  0.5f), -Vector3.UnitY, new Vector2(0, 1)),
            new STDVertex(new Vector3(-0.5f, 0,  0.5f), new Vector3( 0, 0.5f,  1), new Vector2(0, 0)),
            new STDVertex(new Vector3( 0.5f, 0,  0.5f), new Vector3( 0, 0.5f,  1), new Vector2(1, 0)),
            new STDVertex(new Vector3( 0,    1,  0   ), new Vector3( 0, 0.5f,  1), new Vector2(0.5f, 1)),
            new STDVertex(new Vector3( 0.5f, 0, -0.5f), new Vector3( 0, 0.5f, -1), new Vector2(0, 0)),
            new STDVertex(new Vector3(-0.5f, 0, -0.5f), new Vector3( 0, 0.5f, -1), new Vector2(1, 0)),
            new STDVertex(new Vector3( 0,    1,  0   ), new Vector3( 0, 0.5f, -1), new Vector2(0.5f, 1)),
            new STDVertex(new Vector3(-0.5f, 0, -0.5f), new Vector3(-1, 0.5f,  0), new Vector2(0, 0)),
            new STDVertex(new Vector3(-0.5f, 0,  0.5f), new Vector3(-1, 0.5f,  0), new Vector2(1, 0)),
            new STDVertex(new Vector3( 0,    1,  0   ), new Vector3(-1, 0.5f,  0), new Vector2(0.5f, 1)),
            new STDVertex(new Vector3( 0.5f, 0,  0.5f), new Vector3( 1, 0.5f,  0), new Vector2(0, 0)),
            new STDVertex(new Vector3( 0.5f, 0, -0.5f), new Vector3( 1, 0.5f,  0), new Vector2(1, 0)),
            new STDVertex(new Vector3( 0,    1,  0   ), new Vector3( 1, 0.5f,  0), new Vector2(0.5f, 1)),
        };
        uint[] indices = {
            0, 1, 2, 2, 3, 0,  // Base
            4, 5, 6,            // Front
            7, 8, 9,            // Back
            10, 11, 12,         // Left
            13, 14, 15,         // Right
        };
        return (vertices, indices);
    }

    private (STDVertex[], uint[]) GetCircle()
    {
        int segments = 32; float radius = 0.5f;
        List<STDVertex> vertices = new List<STDVertex>();
        List<uint> indices = new List<uint>();
        vertices.Add(new STDVertex(Vector3.Zero, Vector3.UnitZ, new Vector2(0.5f, 0.5f)));
        for (int i = 0; i <= segments; i++)
        {
            float angle = (float)i / segments * System.MathF.PI * 2f;
            float x = System.MathF.Cos(angle) * radius;
            float y = System.MathF.Sin(angle) * radius;
            vertices.Add(new STDVertex(new Vector3(x, y, 0), Vector3.UnitZ, new Vector2(x + 0.5f, y + 0.5f)));
        }
        for (int i = 0; i < segments; i++)
        {
            indices.Add(0); indices.Add((uint)(i + 1)); indices.Add((uint)(i + 2));
        }
        return (vertices.ToArray(), indices.ToArray());
    }

}
