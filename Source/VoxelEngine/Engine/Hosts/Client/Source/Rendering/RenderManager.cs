using System.Numerics;
using VoxelEngine.Core;
using VoxelEngine.Graphics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace VoxelEngine.Client.Rendering;

public sealed class RenderManager
{
    private readonly RenderGraph _renderGraph;
    private readonly CamerasRegistries _camerasRegistries;

    private List<RenderCommand> _renderCommands = new(10000);
    // public IReadOnlyList<RenderCommand> RenderCommands => _renderCommands;

    private IGraphicsDevice device;

    public RenderManager()
    {
        _renderGraph = new RenderGraph();
        _renderCommands = new List<RenderCommand>();

        _camerasRegistries = new CamerasRegistries();
        ServiceContainer.Register(_camerasRegistries);

        device = ServiceContainer.Get<GraphicsContext>()!.Device;

        _renderGraph.CameraBuffer = device.Factory.CreateBuffer(new BufferDescription()
        {
            Size = (uint)Unsafe.SizeOf<CameraData>(),
            Usage = BufferUsage.UniformBuffer
        });
    }

    internal void Initialize()
    {
        _renderGraph.Initialize();

        C_Camera camera = new C_Camera(CameraProjectionType.Perspective);
        camera.UpdateView(new Vector3(0, 0, 3), Vector3.Zero, Vector3.UnitY);
        _camerasRegistries.Add(new CameraData(camera.View, camera.Projection, camera.Priority));

        STDVertex[] vertices = new STDVertex[]
        {
             new STDVertex() { Position = new Vector3(0.0f,  0.5f, 0.0f) },
             new STDVertex() { Position = new Vector3(0.5f, -0.5f, 0.0f) },
             new STDVertex() { Position = new Vector3(-0.5f, -0.5f, 0.0f) }
        };
        uint[] indices = new uint[] { 0, 1, 2 };

        STDMeshData meshData = new STDMeshData(vertices, indices);
        MeshHandle triangleHandle = device.Factory.CreateMesh(meshData);
        MeshAsset.HardcodedTriangleAsset = new MeshAsset() { Handle = triangleHandle, VertexCount = 3, IndexCount = 3 };

        // Pre-allocate a test render command here, simulating what GameWorld ECS would produce.
        var _material = new PBRMaterial(new PBRMaterialProperties() { Color = Color.Orange });
        PipelineHandle pipeline = device.Factory.CreatePipeline(new PipelineDescription()
        {
            VertexShaderSource = @"
#version 460 core
layout(location = 0) in vec3 aPosition;

layout(std140, binding = 0) uniform CameraBlock {
    mat4 view;
    mat4 projection;
} camera;

void main()
{
    gl_Position = camera.projection * camera.view * vec4(aPosition, 1.0);
}
",
            FragmentShaderSource = @"
#version 460 core
layout(std140, binding = 1) uniform PBRMaterialProperties {
    vec4 Color;
} material;

out vec4 FragColor;
void main()
{
    FragColor = material.Color;
}"
        });
        _material.Pipeline = pipeline;

        _renderCommands.Add(new RenderCommand(MeshAsset.HardcodedTriangleAsset, _material, Matrix4x4.Identity));
    }

    internal void Submit(RenderCommand renderCommand) => _renderCommands.Add(renderCommand);
    internal void Submit(ReadOnlySpan<RenderCommand> renderCommands) => _renderCommands.AddRange(renderCommands);

    public void Render()
    {
        // 1. Process active main camera and flush the UBO
        if (_camerasRegistries.Cameras.Count > 0)
        {
            var activeCamera = _camerasRegistries.Cameras.First();
            var cmdList = device.Factory.CreateCommandsList();
            cmdList.Begin();
            cmdList.UpdateBuffer(_renderGraph.CameraBuffer, 0, ref activeCamera);
            cmdList.BindUniformBuffer(_renderGraph.CameraBuffer, 0); // Slot 0
            cmdList.End();
            device.Submit(cmdList);
        }

        // 2. Adjust dynamic test rendering elements
        if (_renderCommands.Count > 0)
        {
            var r = (float)(EMath.Sin(((float)Time.TotalTime) * 2f) * 0.5f + 0.5f);
            var g = (float)(EMath.Sin(((float)Time.TotalTime) * 3f) * 0.5f + 0.5f);
            var b = (float)(EMath.Sin(((float)Time.TotalTime) * 4f) * 0.5f + 0.5f);

            var pbrMaterial = (PBRMaterial)_renderCommands[0].Material;
            pbrMaterial.Properties.Color = new Color(r, g, b, 1.0f);
            pbrMaterial.ApplyChanges(); // Sets dirty flag
        }

        // 3. Dispatch graph
        _renderGraph.Execute(CollectionsMarshal.AsSpan(_renderCommands));

        device.Render();
        device.Present();

        // _renderCommands.Clear();
    }

}