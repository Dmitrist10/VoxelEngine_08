using System;

namespace VoxelEngine.Packages.Voxel;

public class Biome
{
    public string Name { get; set; } = "Unknown";
    public uint SurfaceBlock { get; set; } = 1;
    public uint SubSurfaceBlock { get; set; } = 2;
    public int SurfaceDepth { get; set; } = 3;

    // Ranging from -1 to 1 based on Noise
    public float MinTemperature { get; set; } = -1f;
    public float MaxTemperature { get; set; } = 1f;
    public float MinHumidity { get; set; } = -1f;
    public float MaxHumidity { get; set; } = 1f;

    // Terrain Shape
    public int BaseHeight { get; set; } = 30;
    public float ContinentalnessScale { get; set; } = 30f;
    public float ErosionSpikePower { get; set; } = 2.5f;
    public int MaxHeightVariation { get; set; } = 80;

    public Biome() { }

    public Biome(string name, uint surfaceBlock, uint subSurfaceBlock, int surfaceDepth,
                 float minTemp, float maxTemp, float minHumidity, float maxHumidity)
    {
        Name = name;
        SurfaceBlock = surfaceBlock;
        SubSurfaceBlock = subSurfaceBlock;
        SurfaceDepth = surfaceDepth;
        MinTemperature = minTemp;
        MaxTemperature = maxTemp;
        MinHumidity = minHumidity;
        MaxHumidity = maxHumidity;
    }

    public bool Matches(float temperature, float humidity)
    {
        return temperature >= MinTemperature && temperature <= MaxTemperature &&
               humidity >= MinHumidity && humidity <= MaxHumidity;
    }
}
