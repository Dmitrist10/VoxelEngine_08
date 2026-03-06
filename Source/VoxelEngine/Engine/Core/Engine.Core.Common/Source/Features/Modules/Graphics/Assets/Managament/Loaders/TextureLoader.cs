using StbImageSharp;
using System.IO;
using VoxelEngine.Diagnostics;

namespace VoxelEngine.Core.Assets;

public class TextureLoader : IAssetLoader
{
    /// <summary>
    /// Default load - uses PixelArt preset (Nearest filtering, flipped for OpenGL)
    /// </summary>
    public IAssetData Load(string path)
    {
        return LoadWithOptions(path, TextureOptions.PixelArt);
    }

    public TextureData LoadForIcon(string path)
    {
        return LoadWithOptions(path, TextureOptions.Icon);
    }

    /// <summary>
    /// Load with custom options
    /// </summary>
    public TextureData LoadWithOptions(string path, TextureOptions options)
    {
        byte[] fileData = File.ReadAllBytes(path);
        ImageResult image = ImageResult.FromMemory(fileData, ColorComponents.RedGreenBlueAlpha);

        if (image.Data == null || image.Data.Length == 0)
        {
            Logger.Error($"[TextureLoader] Failed to load {path} or empty.");
            return new TextureData([255, 0, 255, 255], 1, 1, options); // Magenta error texture
        }

        byte[] pixelData = image.Data;

        if (options.FlipVertically)
        {
            pixelData = FlipVertically(image.Data, image.Width, image.Height);
        }

        return new TextureData(pixelData, (uint)image.Width, (uint)image.Height, options);
    }

    /// <summary>
    /// Flip image data vertically (required for OpenGL texture upload)
    /// </summary>
    private static byte[] FlipVertically(byte[] data, int width, int height)
    {
        int bytesPerRow = width * 4; // RGBA = 4 bytes per pixel
        byte[] flipped = new byte[data.Length];

        for (int y = 0; y < height; y++)
        {
            int srcRow = y * bytesPerRow;
            int dstRow = (height - 1 - y) * bytesPerRow;
            Array.Copy(data, srcRow, flipped, dstRow, bytesPerRow);
        }

        return flipped;
    }
}
