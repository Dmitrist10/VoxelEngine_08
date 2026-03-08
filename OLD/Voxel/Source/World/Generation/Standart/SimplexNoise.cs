using System;

namespace VoxelEngine.Packages.Voxel;

public class SimplexNoise
{
    private readonly int[] p = new int[512];

    public SimplexNoise(int seed = 0)
    {
        Random rand = new Random(seed);
        int[] permutation = new int[256];
        for (int i = 0; i < 256; i++)
            permutation[i] = i;

        for (int i = 0; i < 256; i++)
        {
            int j = rand.Next(256);
            (permutation[i], permutation[j]) = (permutation[j], permutation[i]);
        }

        for (int i = 0; i < 256; i++)
        {
            p[i] = permutation[i];
            p[i + 256] = permutation[i];
        }
    }

    /// <summary>
    /// Returns a noise value roughly between -1.0 and 1.0
    /// </summary>
    public float CalcPixel2D(float x, float y, float scale = 1f)
    {
        return Perlin(x * scale, y * scale);
    }

    /// <summary>
    /// Fractal Brownian Motion for more realistic terrain
    /// </summary>
    public float FBM(float x, float y, int octaves, float scale = 1f, float persistence = 0.5f, float lacunarity = 2f)
    {
        float total = 0;
        float frequency = scale;
        float amplitude = 1;
        float maxValue = 0;

        for (int i = 0; i < octaves; i++)
        {
            total += Perlin(x * frequency, y * frequency) * amplitude;
            maxValue += amplitude;
            amplitude *= persistence;
            frequency *= lacunarity;
        }

        return total / maxValue;
    }

    private float Perlin(float x, float y)
    {
        int X = (int)Math.Floor(x) & 255;
        int Y = (int)Math.Floor(y) & 255;

        x -= (float)Math.Floor(x);
        y -= (float)Math.Floor(y);

        float u = Fade(x);
        float v = Fade(y);

        int A = p[X] + Y, B = p[X + 1] + Y;

        float res = Lerp(v, Lerp(u, Grad(p[A], x, y),
                                     Grad(p[B], x - 1, y)),
                            Lerp(u, Grad(p[A + 1], x, y - 1),
                                     Grad(p[B + 1], x - 1, y - 1)));

        return res;
    }

    private float Fade(float t) => t * t * t * (t * (t * 6 - 15) + 10);
    private float Lerp(float t, float a, float b) => a + t * (b - a);
    private float Grad(int hash, float x, float y)
    {
        int h = hash & 15;
        float u = h < 8 ? x : y;
        float v = h < 4 ? y : h == 12 || h == 14 ? x : 0f;
        return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
    }
}
