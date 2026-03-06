using VoxelEngine.Core;
using VoxelEngine.Graphics;
using VoxelEngine.Graphics.Veldrid;

namespace VoxelEngine.Runtime.Platforms.Desktop;

/// <summary>
/// Desktop platform using the Veldrid graphics backend.
/// Silk.NET.Windowing.Extensions.Veldrid auto-selects the best API:
///   Windows  → Direct3D 11 or Vulkan
///   Linux    → Vulkan or OpenGL
///   macOS    → Metal
/// The same IGraphicsDevice code runs on all three.
/// </summary>
public sealed class Windows_VeldridPlatform : IPlatform
{
    public GraphicsContext CreateGraphicsContext()
    {
        // Reuse the existing DesktopWindow — Veldrid wraps the same IWindow
        IWindowSurface window = new DesktopWindow("Voxel Engine [Veldrid]", 800, 600);
        IGraphicsDevice graphicsDevice = new VEL_GraphicsDevice(window);

        return new GraphicsContext(window, graphicsDevice);
    }
}
