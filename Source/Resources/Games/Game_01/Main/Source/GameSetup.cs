using System.Numerics;
using VoxelEngine.Core;
using VoxelEngine.Diagnostics;
using VoxelEngine.Graphics;

namespace CustomGame;

public static class GameSetup
{

    public static void SetUp(Scene scene)
    {
        Logger.Debug("GameSetup.SetUp was called!");

        IGraphicsDevice device = ServiceContainer.Get<GraphicsContext>()!.Device;

        STDVertex[] vertices = new STDVertex[]
                {
             new STDVertex() { Position = new Vector3(0.0f,  0.5f, 0.0f) },
             new STDVertex() { Position = new Vector3(0.5f, -0.5f, 0.0f) },
             new STDVertex() { Position = new Vector3(-0.5f, -0.5f, 0.0f) }
                };

        uint[] indices = new uint[] { 0, 1, 2 };

        STDMeshData meshData = new STDMeshData(vertices, indices);
        MeshHandle triangleHandle = device.Factory.CreateMesh(meshData);
        MeshAsset triangleAsset = new MeshAsset() { Handle = triangleHandle, VertexCount = 3, IndexCount = 3 };

        ShaderData shaderData = ServiceContainer.Get<AssetsManager>()!.LoadShaderData(@"C:\Users\Dmitrist10\Desktop\VoxelGames\Source\Project_08_VE\Source\VoxelEngine\Engine\Core\Engine.Core.Common\Resources\Shaders\UberShader.glsl");

        PipelineHandle pipeline = device.Factory.CreatePipeline(new PipelineDescription()
        {
            VertexShaderSource = shaderData.Vert,
            FragmentShaderSource = shaderData.Frag,
            // layout(location = 1) in vec3 aNormal;
            //             VertexShaderSource = @"
            // #version 460 core
            // layout(location = 0) in vec3 aPosition;

            // layout(std140, binding = 0) uniform CameraBlock {
            //     mat4 view;
            //     mat4 projection;
            // } camera;

            // layout(std140, binding = 2) uniform ModelBlock {
            //     mat4 model;
            // } modelData;

            // void main()
            // {
            //     gl_Position = camera.projection * camera.view * modelData.model * vec4(aPosition, 1.0);
            // }
            // ",
            //             FragmentShaderSource = @"
            // #version 460 core
            // layout(std140, binding = 1) uniform PBRMaterialProperties {
            //     vec4 Color;
            // } material;

            // out vec4 FragColor;
            // void main()
            // {
            //     FragColor = material.Color;
            // }"
        });

        var mat1 = new PBRMaterial(new PBRMaterialProperties() { Color = Color.Orange })
        {
            Pipeline = pipeline
        };

        Actor cubeActor = scene.CreateActor();
        cubeActor.AddComponent(new C_Mesh(triangleAsset, mat1));
        cubeActor.AddComponent<C_ColorChanger>();
        cubeActor.Position = new Vector3(0, 0, 0);

        // Camera
        Actor cameraActor = scene.CreateActor();
        C_Camera camera = new C_Camera(CameraProjectionType.Perspective);
        cameraActor.AddComponent(camera);
        cameraActor.Position = new Vector3(0, 0, -10);

        scene.AddProcessor(new EP_ColorChanger());

        // for (int i = -50; i < 50; i++)
        // {
        //     Actor a = scene.CreateActor();
        //     a.AddComponent(new C_Mesh(triangleAsset, new PBRMaterial(new PBRMaterialProperties() { Color = Color.Orange }) { Pipeline = pipeline }));
        //     a.AddComponent<C_ColorChanger>();
        //     a.Position = new Vector3(i * 0.75f, i * 0.1f, i * 0.5f);
        // }
    }

}