namespace VoxelEngine.Core;

public record struct TextureAsset : IAsset
{
    public TextureHandle Handle;
    public uint Width;
    public uint Height;
}

public record TextureData
{
    public readonly byte[] Data;
    public readonly uint Width;
    public readonly uint Height;
    public readonly uint Channels;
    public readonly TextureOptions Options;

    public TextureData(byte[] data, uint width, uint height, uint channels, TextureOptions options)
    {
        Data = data;
        Width = width;
        Height = height;
        Channels = channels;
        Options = options;
    }
}

/// <summary>
/// Texture filtering mode - determines how textures look when scaled
/// </summary>
public enum TextureFilterMode
{
    /// <summary>
    /// Sharp pixels - ideal for pixel art, voxel games, icons (no blur)
    /// </summary>
    Nearest,

    /// <summary>
    /// Smooth interpolation - ideal for photos, realistic textures (blurs when scaled)
    /// </summary>
    Linear,

    /// <summary>
    /// Linear with mipmaps - best for 3D textures viewed at various distances
    /// </summary>
    LinearMipmap
}

/// <summary>
/// Texture wrap mode - determines what happens at texture edges
/// </summary>
public enum TextureWrapMode
{
    /// <summary>
    /// Repeats the texture (good for tiling)
    /// </summary>
    Repeat,

    /// <summary>
    /// Clamps to edge pixels (no repeat)
    /// </summary>
    ClampToEdge,

    /// <summary>
    /// Mirrors the texture on repeat
    /// </summary>
    MirroredRepeat
}

public record TextureOptions
{
    /// <summary>
    /// Flip texture vertically (required for OpenGL, not for window icons)
    /// </summary>
    public bool FlipVertically { get; set; } = false;

    /// <summary>
    /// Filtering mode - Nearest for pixel art, Linear for smooth textures
    /// </summary>
    public TextureFilterMode FilterMode { get; set; } = TextureFilterMode.Nearest;

    /// <summary>
    /// Wrap mode for texture edges
    /// </summary>
    public TextureWrapMode WrapMode { get; set; } = TextureWrapMode.Repeat;

    /// <summary>
    /// Generate mipmaps (improves quality at distance, increases memory)
    /// </summary>
    public bool GenerateMipmaps { get; set; } = false;

    // Common presets
    public static TextureOptions PixelArt => new()
    {
        FlipVertically = true,
        FilterMode = TextureFilterMode.Nearest,
        WrapMode = TextureWrapMode.Repeat,
        GenerateMipmaps = false
    };

    public static TextureOptions Smooth => new()
    {
        FlipVertically = true,
        FilterMode = TextureFilterMode.Linear,
        WrapMode = TextureWrapMode.ClampToEdge,
        GenerateMipmaps = true
    };

    public static TextureOptions Icon => new()
    {
        FlipVertically = false, // Icons don't need flip
        FilterMode = TextureFilterMode.Nearest,
        WrapMode = TextureWrapMode.ClampToEdge,
        GenerateMipmaps = false
    };

    public static TextureOptions VoxelAtlas => new()
    {
        FlipVertically = true,
        FilterMode = TextureFilterMode.Nearest, // Sharp blocks
        WrapMode = TextureWrapMode.Repeat, // For tiling
        GenerateMipmaps = true // Reduces shimmer at distance
    };
}