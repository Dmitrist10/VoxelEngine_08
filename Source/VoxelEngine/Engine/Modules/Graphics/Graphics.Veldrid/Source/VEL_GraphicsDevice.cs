using System.Numerics;

using Silk.NET.Windowing;
using Silk.NET.Windowing.Extensions.Veldrid;

using Veldrid;

using VoxelEngine.Diagnostics;

namespace VoxelEngine.Graphics.Veldrid;

public sealed class VEL_GraphicsDevice : IGraphicsDevice
{
    public IGraphicsFactory Factory { get; private set; } = null!;

    private readonly IWindowSurface _surface;

    // Veldrid core objects — created after the window is ready
    private GraphicsDevice _device = null!;
    private ResourceFactory _velFactory = null!;
    private Swapchain _swapchain = null!;
    private VEL_AssetsManager _assetsManager = null!;

    private readonly List<VEL_GraphicsCommandsList> _commandLists = new();

    /// <param name="surface">
    /// The platform window surface. On Desktop this wraps a Silk.NET <see cref="IWindow"/>.
    /// On Android it would wrap an ANativeWindow — the rest of the engine doesn't care.
    /// </param>
    public VEL_GraphicsDevice(IWindowSurface surface)
    {
        _surface = surface;
        _surface.OnLoad += OnWindowLoad;
        _surface.OnResize += OnWindowResize;
    }

    // -------------------------------------------------------------------------
    // Window events
    // -------------------------------------------------------------------------

    private void OnWindowLoad()
    {
        Logger.Info("[Veldrid] Initialising graphics device...");

        // The Silk.NET.Windowing.Extensions.Veldrid package provides
        // IWindow.CreateGraphicsDevice() which picks the best backend automatically
        // (Vulkan on Android/Linux, D3D11 on Windows, Metal on macOS).
        IWindow nativeWindow = (_surface.NativeWindow as IWindow)!;

        GraphicsDeviceOptions options = new()
        {
            PreferStandardClipSpaceYDirection = true,
            PreferDepthRangeZeroToOne = true,
            Debug = false,
        };

        _device = nativeWindow.CreateGraphicsDevice(options);
        _swapchain = _device.MainSwapchain;
        _velFactory = _device.ResourceFactory;

        _assetsManager = new VEL_AssetsManager();
        Factory = new VEL_GraphicsFactory(_device, _velFactory, _assetsManager);

        Logger.Info("[Veldrid] Graphics device ready.");
    }

    private void OnWindowResize(Vector2 size)
    {
        _swapchain?.Resize((uint)size.X, (uint)size.Y);
    }

    // -------------------------------------------------------------------------
    // IGraphicsDevice
    // -------------------------------------------------------------------------

    public void Render()
    {
        foreach (VEL_GraphicsCommandsList list in _commandLists)
            list.Execute(_device);

        _commandLists.Clear();
    }

    public void Present()
    {
        _device.SwapBuffers(_swapchain);
    }

    public void Submit(IGraphicsCommandsList list)
    {
        _commandLists.Add((VEL_GraphicsCommandsList)list);
    }

    public void Submit(ReadOnlySpan<IGraphicsCommandsList> lists)
    {
        foreach (IGraphicsCommandsList list in lists)
            _commandLists.Add((VEL_GraphicsCommandsList)list);
    }

    public void Dispose()
    {
        foreach (VEL_GraphicsCommandsList list in _commandLists)
            list.Dispose();

        _swapchain?.Dispose();
        _device?.Dispose();
    }
}
