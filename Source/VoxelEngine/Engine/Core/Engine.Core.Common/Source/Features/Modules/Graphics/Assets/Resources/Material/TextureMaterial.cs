using VoxelEngine.Graphics;

namespace VoxelEngine.Core;

public sealed class TextureMaterial : Material<TextureMaterialProperties>
{
    public TextureHandle AlbedoTexture;

    public TextureMaterial(TextureMaterialProperties properties) : base(properties)
    {
    }

    public TextureMaterial() : base()
    {
    }

    public override void SetRendering(IGraphicsCommandsList cmdBuffer)
    {
        base.SetRendering(cmdBuffer);
        cmdBuffer.BindTexture(AlbedoTexture, 0);
    }
}

public record struct TextureMaterialProperties : IMaterialProperties
{
    public Color Color;

    public TextureMaterialProperties()
    {
        Color = Color.White;
    }

    public TextureMaterialProperties(Color color)
    {
        Color = color;
    }
}
