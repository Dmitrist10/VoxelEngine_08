using System.Numerics;
using VoxelEngine.Core;
using VoxelEngine.Graphics;

namespace CustomGame;

internal class BuildInAssets
{
    internal static PBRMaterial GetPBRMaterial()
    {
        IGraphicsDevice device = ServiceContainer.Get<GraphicsContext>()!.Device;
        var manager = ServiceContainer.Get<AssetsManager>()!;

        ShaderData vertData = manager.Load<ShaderData>("vfs://builtin/Shaders/UberShader.vert").Get();
        ShaderData fragData = manager.Load<ShaderData>("vfs://builtin/Shaders/UberShader.frag").Get();

        PipelineHandle pipeline = device.Factory.CreatePipeline(new PipelineDescription()
        {
            VertexShaderSource = vertData.Vert,
            FragmentShaderSource = fragData.Frag,
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
        var manager = ServiceContainer.Get<AssetsManager>()!;

        // Using standard shaders for texture, temporarily use UberShader or custom if available
        // Assuming TextureShader.glsl is on disk at Game_01 or Engine level.
        // We will map exactly the disk path that the user previously had!
        string shaderPath = @"C:\Users\Dmitrist10\Desktop\VoxelGames\Source\Project_08_VE\Source\VoxelEngine\Engine\Core\Engine.Core.Common\Resources\Shaders\TextureShader.glsl";
        ShaderData shaderData = manager.Load<ShaderData>($"vfs://disk/{shaderPath.Replace('\\', '/')}").Get();

        PipelineHandle pipeline = device.Factory.CreatePipeline(new PipelineDescription()
        {
            VertexShaderSource = shaderData.Vert,
            FragmentShaderSource = shaderData.Frag,
        });

        string texPath = @"C:\Users\Dmitrist10\Desktop\VoxelGames\Source\Project_08_VE\Source\Resources\Games\Game_01\Assets\Models\Tree\Tree.png";
        TextureData texData = manager.Load<TextureData>($"vfs://disk/{texPath.Replace('\\', '/')}").Get();
        TextureHandle texHandle = device.Factory.CreateTexture(texData);

        var mat = new TextureMaterial(new TextureMaterialProperties() { Color = Color.White })
        {
            Pipeline = pipeline,
            AlbedoTexture = texHandle
        };

        return mat;
    }

    internal static MeshAsset GetBuiltInMesh(string primitiveName)
    {
        var manager = ServiceContainer.Get<AssetsManager>()!;
        var graphics = ServiceContainer.Get<GraphicsContext>()!;

        STDMeshData meshData = manager.Load<STDMeshData>($"vfs://builtin/Primitives/{primitiveName}.ve_mesh").Get();
        MeshHandle handle = graphics.Device.Factory.CreateMesh(meshData);

        return new MeshAsset()
        {
            Handle = handle,
            VertexCount = meshData.VertexCount,
            IndexCount = meshData.IndexCount,
        };
    }
}