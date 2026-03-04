namespace VoxelEngine.Core;

public record struct ShaderAsset : IAsset
{
    public ShaderHandle Handle;
}


public record ShaderData
{
    public readonly string Vert;
    public readonly string Frag;

    public ShaderData(string vert, string frag)
    {
        Vert = vert;
        Frag = frag;
    }
}


public record struct PipelineAsset : IAsset
{
    public PipelineHandle Handle;
}

public record PipelineData
{
    public readonly ShaderData Shader;
}



public enum PrimitiveTopology
{
    Triangles,
    Lines,
    LineStrip
}