using Veldrid;
using Veldrid.SPIRV;

using VoxelEngine.Core;

using VelPrimTopo = Veldrid.PrimitiveTopology;
using VelBufDesc = Veldrid.BufferDescription;
using VelBufUsage = Veldrid.BufferUsage;

namespace VoxelEngine.Graphics.Veldrid;

internal sealed class VEL_GraphicsFactory : IGraphicsFactory
{
    private readonly GraphicsDevice _device;
    private readonly ResourceFactory _factory;
    private readonly VEL_AssetsManager _assets;

    public VEL_GraphicsFactory(GraphicsDevice device, ResourceFactory factory, VEL_AssetsManager assets)
    {
        _device = device;
        _factory = factory;
        _assets = assets;
    }

    // -------------------------------------------------------------------------
    // Command lists
    // -------------------------------------------------------------------------

    public IGraphicsCommandsList CreateCommandsList()
    {
        CommandList cl = _factory.CreateCommandList();
        return new VEL_GraphicsCommandsList(cl, _assets);
    }

    // -------------------------------------------------------------------------
    // Buffers
    // -------------------------------------------------------------------------

    public BufferHandle CreateBuffer(in BufferDescription description)
    {
        VelBufUsage usage = description.Usage switch
        {
            BufferUsage.VertexBuffer => VelBufUsage.VertexBuffer,
            BufferUsage.IndexBuffer => VelBufUsage.IndexBuffer,
            BufferUsage.UniformBuffer => VelBufUsage.UniformBuffer | VelBufUsage.Dynamic,
            _ => VelBufUsage.VertexBuffer
        };

        uint size = description.Size;
        if (description.Usage == BufferUsage.UniformBuffer)
        {
            // Veldrid requires uniform buffer sizes to be a multiple of 16 bytes
            size = (size + 15) & ~15u;
        }

        DeviceBuffer buffer = _factory.CreateBuffer(
            new VelBufDesc(size, usage));

        if (description.Data != 0)
        {
            _device.UpdateBuffer(buffer, 0, description.Data, description.Size);
        }

        return _assets.Add(new VEL_Buffer(buffer, description.Size));
    }

    // -------------------------------------------------------------------------
    // Pipelines  — GLSL cross-compiled to SPIR-V at runtime via Veldrid.SPIRV
    // -------------------------------------------------------------------------

    public PipelineHandle CreatePipeline(in PipelineDescription description)
    {
        // --- 1. Compile GLSL → SPIR-V (Veldrid.SPIRV handles the cross-compile
        //        to whatever native shading language the active backend needs)
        ShaderDescription vertDesc = new(
            ShaderStages.Vertex,
            System.Text.Encoding.UTF8.GetBytes(description.VertexShaderSource),
            "main");

        ShaderDescription fragDesc = new(
            ShaderStages.Fragment,
            System.Text.Encoding.UTF8.GetBytes(description.FragmentShaderSource),
            "main");

        Shader[] shaders = _factory.CreateFromSpirv(vertDesc, fragDesc);

        // --- 2. Resource layout (uniform buffer at binding slot 0)
        ResourceLayout resourceLayout = _factory.CreateResourceLayout(
            new ResourceLayoutDescription(
                new ResourceLayoutElementDescription(
                    "MaterialUBO",
                    ResourceKind.UniformBuffer,
                    ShaderStages.Vertex | ShaderStages.Fragment)));

        // --- 3. Vertex layout derived from IVertexType metadata ---
        // Note: vertex layout is generic-erased here; for multi-vertex-format
        // support the PipelineDescription should carry layout info.
        // For now we use an empty layout (no vertex input) as a safe default
        // so at least the pipeline compiles. Meshes pass layout via VAO in GL;
        // in Veldrid the layout is declared on the pipeline.
        // TODO: extend PipelineDescription with VertexLayoutDescription.
        VertexLayoutDescription vertexLayout = new(
            Array.Empty<VertexElementDescription>());

        // --- 4. Full pipeline description ---
        GraphicsPipelineDescription pipelineDesc = new()
        {
            BlendState = BlendStateDescription.SingleOverrideBlend,
            DepthStencilState = new DepthStencilStateDescription(
                depthTestEnabled: true,
                depthWriteEnabled: true,
                comparisonKind: ComparisonKind.LessEqual),
            RasterizerState = new RasterizerStateDescription(
                cullMode: FaceCullMode.Back,
                fillMode: PolygonFillMode.Solid,
                frontFace: FrontFace.Clockwise,
                depthClipEnabled: true,
                scissorTestEnabled: false),
            PrimitiveTopology = VelPrimTopo.TriangleList,
            ResourceLayouts = [resourceLayout],
            ShaderSet = new ShaderSetDescription(
                vertexLayouts: [vertexLayout],
                shaders: shaders),
            Outputs = _device.SwapchainFramebuffer.OutputDescription,
        };

        Pipeline pipeline = _factory.CreateGraphicsPipeline(pipelineDesc);

        return _assets.Add(new VEL_Pipeline(pipeline, resourceLayout, shaders));
    }

    // -------------------------------------------------------------------------
    // Meshes
    // -------------------------------------------------------------------------

    public MeshHandle CreateMesh<T>(MeshData<T> meshData) where T : unmanaged, IVertexType
    {
        // Vertex buffer
        uint vbSize = (uint)(meshData.Vertices.Length * System.Runtime.CompilerServices.Unsafe.SizeOf<T>());
        DeviceBuffer vb = _factory.CreateBuffer(new VelBufDesc(vbSize, VelBufUsage.VertexBuffer));
        unsafe
        {
            fixed (T* vData = meshData.Vertices)
                _device.UpdateBuffer(vb, 0, (nint)vData, vbSize);
        }

        // Index buffer
        uint ibSize = (uint)(meshData.Indices.Length * sizeof(uint));
        DeviceBuffer ib = _factory.CreateBuffer(new VelBufDesc(ibSize, VelBufUsage.IndexBuffer));
        unsafe
        {
            fixed (uint* iData = meshData.Indices)
                _device.UpdateBuffer(ib, 0, (nint)iData, ibSize);
        }

        return _assets.Add(new VEL_Mesh(vb, ib, (uint)meshData.Indices.Length));
    }

    public TextureHandle CreateTexture(TextureData textureData)
    {
        throw new NotImplementedException();
    }

    public void Dispose() { }

    public TextureHandle CreateTextureArray(Texture2DArrayData textureData)
    {
        throw new NotImplementedException();
    }

    public void DestroyMesh(MeshHandle handle)
    {
        throw new NotImplementedException();
    }

    public void DestroyTexture(TextureHandle handle)
    {
        throw new NotImplementedException();
    }

    public void DestroyBuffer(BufferHandle handle)
    {
        throw new NotImplementedException();
    }
}
