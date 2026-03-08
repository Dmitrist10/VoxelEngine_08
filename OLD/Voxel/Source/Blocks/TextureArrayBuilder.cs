using System;
using System.IO;
using StbImageSharp;
using VoxelEngine.Diagnostics;

namespace VoxelEngine.Core;

public static class TextureArrayBuilder
{
    public static Texture2DArrayData Build(string[] filePaths, TextureOptions options)
    {
        if (filePaths == null || filePaths.Length == 0)
            throw new ArgumentException("File paths array cannot be null or empty.");

        int width = 0;
        int height = 0;
        byte[]? finalBuffer = null;
        int bytesPerLayer = 0;

        for (int i = 0; i < filePaths.Length; i++)
        {
            string path = filePaths[i];

            if (!File.Exists(path))
            {
                Logger.Error($"[TextureArrayBuilder] File not found: {path}");
                continue;
            }

            byte[] fileData = File.ReadAllBytes(path);
            ImageResult image = ImageResult.FromMemory(fileData, ColorComponents.RedGreenBlueAlpha);

            if (image.Data == null || image.Data.Length == 0)
            {
                Logger.Error($"[TextureArrayBuilder] Failed to decode image: {path}");
                continue;
            }

            // Initialize dimensions based on the first successfully loaded image
            if (i == 0)
            {
                width = image.Width;
                height = image.Height;
                bytesPerLayer = width * height * 4; // RGBA
                finalBuffer = new byte[bytesPerLayer * filePaths.Length];
            }
            else
            {
                if (image.Width != width || image.Height != height)
                {
                    // Logger.Warn($"[TextureArrayBuilder] Resizing {path} from {image.Width}x{image.Height} to {width}x{height}");
                    image = ResizeImage(image, width, height);
                }
            }

            byte[] pixelData = image.Data;

            if (options.FlipVertically)
            {
                pixelData = FlipVertically(pixelData, width, height);
            }

            // Copy layer data into the continuous buffer
            Array.Copy(pixelData, 0, finalBuffer!, i * bytesPerLayer, bytesPerLayer);
        }

        if (finalBuffer == null)
            throw new Exception("Failed to build Texture2DArray. No valid images were loaded.");

        return new Texture2DArrayData((uint)width, (uint)height, (uint)filePaths.Length, finalBuffer, options);
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

    /// <summary>
    /// Nearest-neighbor resize of an RGBA image to the target dimensions.
    /// </summary>
    private static ImageResult ResizeImage(ImageResult source, int targetWidth, int targetHeight)
    {
        byte[] resized = new byte[targetWidth * targetHeight * 4];

        for (int y = 0; y < targetHeight; y++)
        {
            int srcY = y * source.Height / targetHeight;
            for (int x = 0; x < targetWidth; x++)
            {
                int srcX = x * source.Width / targetWidth;
                int srcIndex = (srcY * source.Width + srcX) * 4;
                int dstIndex = (y * targetWidth + x) * 4;

                resized[dstIndex] = source.Data[srcIndex];
                resized[dstIndex + 1] = source.Data[srcIndex + 1];
                resized[dstIndex + 2] = source.Data[srcIndex + 2];
                resized[dstIndex + 3] = source.Data[srcIndex + 3];
            }
        }

        return new ImageResult
        {
            Width = targetWidth,
            Height = targetHeight,
            Comp = source.Comp,
            SourceComp = source.SourceComp,
            Data = resized
        };
    }
}
