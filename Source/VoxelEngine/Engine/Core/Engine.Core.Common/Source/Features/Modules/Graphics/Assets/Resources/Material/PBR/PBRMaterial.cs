using System.Runtime.InteropServices;
using VoxelEngine.Graphics;

namespace VoxelEngine.Core;

public sealed class PBRMaterial : Material<PBRMaterialProperties>
{
    public PBRMaterial(PBRMaterialProperties properties) : base(properties)
    {

    }
    public PBRMaterial() : base()
    {

    }
}
public sealed class ChunkMaterial : Material<ChunkMaterialProperties>
{
    public TextureHandle AlbedoTexture;

    public ChunkMaterial(ChunkMaterialProperties properties) : base(properties)
    {

    }
    public ChunkMaterial() : base()
    {

    }

    public override void SetRendering(IGraphicsCommandsList cmdBuffer)
    {
        base.SetRendering(cmdBuffer);
        cmdBuffer.BindTexture(AlbedoTexture, 0);
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 16)]
public record struct ChunkMaterialProperties : IMaterialProperties
{
    public Color Color;
    // public float Metallic;
    // public float Roughness;
    // public float AO;

    public ChunkMaterialProperties()
    {
        Color = Color.White;
    }

    public ChunkMaterialProperties(Color color)
    {
        Color = color;
    }

}
