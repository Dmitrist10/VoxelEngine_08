namespace VoxelEngine.Graphics;

public record struct PipelineDescription
{
    public string VertexShaderSource;
    public string FragmentShaderSource;

    public PipelineDescription(string vertexShaderSource, string fragmentShaderSource) : this()
    {
        VertexShaderSource = vertexShaderSource;
        FragmentShaderSource = fragmentShaderSource;
    }
}
