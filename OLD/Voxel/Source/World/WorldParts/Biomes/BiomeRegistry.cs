using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using VoxelEngine.Diagnostics;

namespace VoxelEngine.Packages.Voxel;

public static class BiomeRegistry
{
    private static List<Biome> _biomes = new List<Biome>();
    private static Biome _fallbackBiome = new Biome("Fallback", 2, 3, 4, -1.0f, 1.0f, -1.0f, 1.0f);

    public static IReadOnlyList<Biome> Biomes => _biomes;

    public static void InitializeDefault()
    {
        _biomes.Clear();
        _biomes.Add(new Biome("Desert", 5, 4, 4, 0.4f, 1.0f, -1.0f, 0.5f));      // Hot & Dry
        _biomes.Add(new Biome("Forest", 6, 5, 4, 0.0f, 0.6f, 0.0f, 1.0f));       // Mild & Wet
        _biomes.Add(new Biome("Snow", 7, 6, 3, -1.0f, 0.0f, -1.0f, 1.0f));       // Cold
        _biomes.Add(new Biome("Plains", 8, 7, 3, 0.0f, 1.0f, -0.4f, 0.4f));      // Mild & Average Humidity
        _biomes.Add(new Biome("Wasteland", 2, 3, 4, -1.0f, 1.0f, -1.0f, 1.0f));  // Fallback Biome
    }

    public static void LoadFromJson(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Logger.Warning($"[BiomeRegistry] Biome JSON file not found at {filePath}. Creating default.");
            InitializeDefault();
            SaveToJson(filePath);
            return;
        }

        try
        {
            string json = File.ReadAllText(filePath);
            var loadedBiomes = JsonSerializer.Deserialize<List<Biome>>(json);
            if (loadedBiomes != null && loadedBiomes.Count > 0)
            {
                _biomes = loadedBiomes;
                Logger.Info($"[BiomeRegistry] Loaded {_biomes.Count} biomes from {filePath}");
            }
            else
            {
                Logger.Warning("[BiomeRegistry] Loaded biome list was empty. Using defaults.");
                InitializeDefault();
                SaveToJson(@"C:\Users\Dmitrist10\Desktop\VoxelGames\Source\Project_04_VE\Resources\UGC\Games\Game_01\Assets\");
            }
        }
        catch (System.Exception ex)
        {
            Logger.Error($"[BiomeRegistry] Error loading biomes: {ex.Message}");
            InitializeDefault();
        }
    }

    public static void SaveToJson(string filePath)
    {
        if (_biomes.Count == 0) InitializeDefault();

        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(_biomes, options);

            string directory = Path.GetDirectoryName(filePath)!;
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(filePath, json);
            Logger.Info($"[BiomeRegistry] Saved {_biomes.Count} biomes to {filePath}");
        }
        catch (System.Exception ex)
        {
            Logger.Error($"[BiomeRegistry] Error saving biomes: {ex.Message}");
        }
    }

    public static Biome GetBiome(float temp, float humidity)
    {
        if (_biomes.Count == 0) return _fallbackBiome;

        for (int i = 0; i < _biomes.Count; i++)
        {
            if (_biomes[i].Matches(temp, humidity))
                return _biomes[i];
        }
        return _biomes[^1]; // fallback
    }
}
