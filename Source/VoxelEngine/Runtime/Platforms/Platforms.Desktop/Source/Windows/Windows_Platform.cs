// using VoxelEngine.IO;
using VoxelEngine.Core;
using VoxelEngine.Graphics;
using VoxelEngine.Graphics.OpenGL;
// using VoxelEngine.Audio;
// using VoxelEngine.Input;

namespace VoxelEngine.Runtime.Platforms.Desktop;

public sealed class Windows_Platform : IPlatform
{
    public GraphicsContext CreateGraphicsContext()
    {
        IWindowSurface window = new DesktopWindow("Voxel Engine", 800, 600);
        IGraphicsDevice graphicsDevice = new GL_GraphicsDevice(window);

        GraphicsContext graphicsContext = new GraphicsContext(window, graphicsDevice);
        return graphicsContext;
    }

    // public IInputModule CreateInputModule()
    // {
    //     return new InputModule();
    // }

    // public IAudioModule CreateAudioModule()
    // {
    //     return new AudioModule();
    // }
    // public IFileManager CreateFileManager()
    // {
    //     return new FileManager();
    // }
}
