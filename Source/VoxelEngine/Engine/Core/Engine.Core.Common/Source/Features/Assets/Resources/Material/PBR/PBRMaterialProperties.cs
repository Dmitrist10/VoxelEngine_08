using System.Runtime.InteropServices;

namespace VoxelEngine.Core;

[StructLayout(LayoutKind.Sequential, Pack = 16)]
public record struct PBRMaterialProperties : IMaterialProperties
{
    public Color Color;
    // public float Metallic;
    // public float Roughness;
    // public float AO;
}
