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

        // ShaderData vertData = manager.GetShaderData("vfs://builtin/Shaders/UberShader.vert");
        // ShaderData fragData = manager.GetShaderData("vfs://builtin/Shaders/UberShader.frag");
        ShaderData data = manager.GetShaderData(@"C:\Users\Dmitrist10\Desktop\VoxelGames\Source\Project_08_VE\Source\VoxelEngine\Engine\Core\Engine.Core.Common\Resources\Shaders\UberShader.glsl");

        PipelineHandle pipeline = device.Factory.CreatePipeline(new PipelineDescription()
        {
            VertexShaderSource = data.Vert,
            FragmentShaderSource = data.Frag,
        });

        var mat1 = new PBRMaterial(new PBRMaterialProperties() { Color = Color.Orange })
        {
            Pipeline = pipeline
        };

        return mat1;
    }

    internal static TextureMaterial GetTreeTextureMaterial()
    {
        var manager = ServiceContainer.Get<AssetsManager>()!;

        string shaderPath = @"C:\Users\Dmitrist10\Desktop\VoxelGames\Source\Project_08_VE\Source\VoxelEngine\Engine\Core\Engine.Core.Common\Resources\Shaders\TextureShader.glsl";
        ShaderData shaderData = manager.GetShaderData(shaderPath);

        PipelineHandle pipeline = manager.GetOrCreatePipeline(new PipelineDescription(shaderData.Vert, shaderData.Frag));

        TextureAsset texture = manager.GetTexture(@"C:\Users\Dmitrist10\Desktop\VoxelGames\Source\Project_08_VE\Source\Resources\Games\Game_01\Assets\Models\Tree\Tree.png");

        var mat = new TextureMaterial(new TextureMaterialProperties() { Color = Color.White })
        {
            Pipeline = pipeline,
            AlbedoTexture = texture.Handle
        };

        return mat;
    }

    internal static MeshAsset GetBuiltInMesh(string primitiveName)
    {
        return ServiceContainer.Get<AssetsManager>()!.GetMesh($"BuildIn://Primitives/{primitiveName}.ve");
    }

}