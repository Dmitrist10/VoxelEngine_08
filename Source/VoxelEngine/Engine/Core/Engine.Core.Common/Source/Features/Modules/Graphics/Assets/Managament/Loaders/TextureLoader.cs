using StbImageSharp;
using System.IO;
using VoxelEngine.Diagnostics;
using System;

namespace VoxelEngine.Core.Assets;

public class TextureLoader : IAssetLoader<TextureData>
{
    public TextureData Load(Stream stream, AssetId id, string absolutePath)
    {
        // For drag-and-drop, options would ideally be parsed from a `.meta` file.
        // For now, we will assume a default PixelArt preset unless metadata is added later.
        TextureOptions options = TextureOptions.PixelArt;

        using var ms = new MemoryStream();
        stream.CopyTo(ms);
        byte[] fileData = ms.ToArray();

        ImageResult image = ImageResult.FromMemory(fileData, ColorComponents.RedGreenBlueAlpha);

        if (image.Data == null || image.Data.Length == 0)
        {
            Logger.Error($"[TextureLoader] Failed to load {absolutePath} or empty.");
            return new TextureData([255, 0, 255, 255], 1, 1, options); // Magenta error texture
        }

        byte[] pixelData = image.Data;

        if (options.FlipVertically)
        {
            pixelData = FlipVertically(image.Data, image.Width, image.Height);
        }

        return new TextureData(pixelData, (uint)image.Width, (uint)image.Height, options);
    }

    private static byte[] FlipVertically(byte[] data, int width, int height)
    {
        int bytesPerRow = width * 4;
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
