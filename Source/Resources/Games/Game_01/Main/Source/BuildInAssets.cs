using System.Numerics;
using VoxelEngine.Core;
using VoxelEngine.Graphics;

namespace CustomGame;

internal class BuildInAssets
{
    internal static PBRMaterial GetPBRMaterial()
    {
        IGraphicsDevice device = ServiceContainer.Get<GraphicsContext>()!.Device;

        ShaderData shaderData = ServiceContainer.Get<AssetsManager>()!.LoadShaderData(@"C:\Users\Dmitrist10\Desktop\VoxelGames\Source\Project_08_VE\Source\VoxelEngine\Engine\Core\Engine.Core.Common\Resources\Shaders\UberShader.glsl");

        PipelineHandle pipeline = device.Factory.CreatePipeline(new PipelineDescription()
        {
            VertexShaderSource = shaderData.Vert,
            FragmentShaderSource = shaderData.Frag,
        });

        var mat1 = new PBRMaterial(new PBRMaterialProperties() { Color = Color.Orange })
        {
            Pipeline = pipeline
        };

        return mat1;
    }

    internal static TextureMaterial GetTreeTextureMaterial()
    {
        IGraphicsDevice device = ServiceContainer.Get<GraphicsContext>()!.Device;

        ShaderData shaderData = ServiceContainer.Get<AssetsManager>()!.LoadShaderData(@"C:\Users\Dmitrist10\Desktop\VoxelGames\Source\Project_08_VE\Source\VoxelEngine\Engine\Core\Engine.Core.Common\Resources\Shaders\TextureShader.glsl");

        PipelineHandle pipeline = device.Factory.CreatePipeline(new PipelineDescription()
        {
            VertexShaderSource = shaderData.Vert,
            FragmentShaderSource = shaderData.Frag,
        });

        TextureData texData = ServiceContainer.Get<AssetsManager>()!.LoadTexture(@"C:\Users\Dmitrist10\Desktop\VoxelGames\Source\Project_08_VE\Source\Resources\Games\Game_01\Assets\Models\Tree\Tree.png", TextureOptions.PixelArt);
        TextureHandle texHandle = device.Factory.CreateTexture(texData);

        var mat = new TextureMaterial(new TextureMaterialProperties() { Color = Color.White })
        {
            Pipeline = pipeline,
            AlbedoTexture = texHandle
        };

        return mat;
    }

    internal static MeshAsset GetTriangle()
    {
        STDVertex[] vertices =
        {
            new STDVertex(new Vector3(-0.5f, -0.5f, 0), new Vector3(0, 0, 1), new Vector2(0,    0)),
            new STDVertex(new Vector3( 0.5f, -0.5f, 0), new Vector3(0, 0, 1), new Vector2(1,    0)),
            new STDVertex(new Vector3( 0.0f,  0.5f, 0), new Vector3(0, 0, 1), new Vector2(0.5f, 1)),
        };

        uint[] indices = { 0, 1, 2 };

        return CreateMesh(vertices, indices);
    }

    internal static MeshAsset GetCube()
    {
        STDVertex[] vertices =
        {
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

        uint[] indices =
        {
             0,  1,  2,  2,  3,  0,  // Front
             4,  5,  6,  6,  7,  4,  // Back
             8,  9, 10, 10, 11,  8,  // Left
            12, 13, 14, 14, 15, 12,  // Right
            16, 17, 18, 18, 19, 16,  // Top
            20, 21, 22, 22, 23, 20,  // Bottom
        };

        return CreateMesh(vertices, indices);
    }

    internal static MeshAsset GetQuad()
    {
        STDVertex[] vertices =
        {
            new STDVertex(new Vector3(-0.5f, -0.5f, 0), new Vector3(0, 0, 1), new Vector2(0, 0)),
            new STDVertex(new Vector3( 0.5f, -0.5f, 0), new Vector3(0, 0, 1), new Vector2(1, 0)),
            new STDVertex(new Vector3( 0.5f,  0.5f, 0), new Vector3(0, 0, 1), new Vector2(1, 1)),
            new STDVertex(new Vector3(-0.5f,  0.5f, 0), new Vector3(0, 0, 1), new Vector2(0, 1)),
        };

        uint[] indices = { 0, 1, 2, 2, 3, 0 };

        return CreateMesh(vertices, indices);
    }

    internal static MeshAsset GetSphere()
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
                    indices.Add(k1);
                    indices.Add(k2);
                    indices.Add(k1 + 1);
                }
                if (i != (stacks - 1))
                {
                    indices.Add(k1 + 1);
                    indices.Add(k2);
                    indices.Add(k2 + 1);
                }
            }
        }

        return CreateMesh(vertices.ToArray(), indices.ToArray());
    }

    internal static MeshAsset GetCylinder()
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
            indices.Add(bottomCenter);
            indices.Add((uint)(bottomCenter + i + 2));
            indices.Add((uint)(bottomCenter + i + 1));
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
            indices.Add(topCenter);
            indices.Add((uint)(topCenter + i + 1));
            indices.Add((uint)(topCenter + i + 2));
        }

        return CreateMesh(vertices.ToArray(), indices.ToArray());
    }

    internal static MeshAsset GetPyramid()
    {
        STDVertex[] vertices =
        {
            // Base (square, facing down)
            new STDVertex(new Vector3(-0.5f, 0, -0.5f), -Vector3.UnitY, new Vector2(0, 0)),
            new STDVertex(new Vector3( 0.5f, 0, -0.5f), -Vector3.UnitY, new Vector2(1, 0)),
            new STDVertex(new Vector3( 0.5f, 0,  0.5f), -Vector3.UnitY, new Vector2(1, 1)),
            new STDVertex(new Vector3(-0.5f, 0,  0.5f), -Vector3.UnitY, new Vector2(0, 1)),

            // Front face
            new STDVertex(new Vector3(-0.5f, 0,  0.5f), new Vector3( 0, 0.5f,  1), new Vector2(0,    0)),
            new STDVertex(new Vector3( 0.5f, 0,  0.5f), new Vector3( 0, 0.5f,  1), new Vector2(1,    0)),
            new STDVertex(new Vector3( 0,    1,  0   ), new Vector3( 0, 0.5f,  1), new Vector2(0.5f, 1)),

            // Back face
            new STDVertex(new Vector3( 0.5f, 0, -0.5f), new Vector3( 0, 0.5f, -1), new Vector2(0,    0)),
            new STDVertex(new Vector3(-0.5f, 0, -0.5f), new Vector3( 0, 0.5f, -1), new Vector2(1,    0)),
            new STDVertex(new Vector3( 0,    1,  0   ), new Vector3( 0, 0.5f, -1), new Vector2(0.5f, 1)),

            // Left face
            new STDVertex(new Vector3(-0.5f, 0, -0.5f), new Vector3(-1, 0.5f,  0), new Vector2(0,    0)),
            new STDVertex(new Vector3(-0.5f, 0,  0.5f), new Vector3(-1, 0.5f,  0), new Vector2(1,    0)),
            new STDVertex(new Vector3( 0,    1,  0   ), new Vector3(-1, 0.5f,  0), new Vector2(0.5f, 1)),

            // Right face
            new STDVertex(new Vector3( 0.5f, 0,  0.5f), new Vector3( 1, 0.5f,  0), new Vector2(0,    0)),
            new STDVertex(new Vector3( 0.5f, 0, -0.5f), new Vector3( 1, 0.5f,  0), new Vector2(1,    0)),
            new STDVertex(new Vector3( 0,    1,  0   ), new Vector3( 1, 0.5f,  0), new Vector2(0.5f, 1)),
        };

        uint[] indices =
        {
            0, 1, 2, 2, 3, 0,  // Base
            4, 5, 6,            // Front
            7, 8, 9,            // Back
            10, 11, 12,         // Left
            13, 14, 15,         // Right
        };

        return CreateMesh(vertices, indices);
    }

    internal static MeshAsset GetCircle()
    {
        int segments = 32;
        float radius = 0.5f;

        List<STDVertex> vertices = new List<STDVertex>();
        List<uint> indices = new List<uint>();

        // Center vertex
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
            indices.Add(0);
            indices.Add((uint)(i + 1));
            indices.Add((uint)(i + 2));
        }

        return CreateMesh(vertices.ToArray(), indices.ToArray());
    }

    private static MeshAsset CreateMesh(STDVertex[] vertices, uint[] indices)
    {
        IGraphicsDevice device = ServiceContainer.Get<GraphicsContext>()!.Device;
        STDMeshData meshData = new STDMeshData(vertices, indices);
        MeshHandle handle = device.Factory.CreateMesh(meshData);
        return new MeshAsset()
        {
            Handle = handle,
            VertexCount = (uint)vertices.Length,
            IndexCount = (uint)indices.Length,
        };
    }

}