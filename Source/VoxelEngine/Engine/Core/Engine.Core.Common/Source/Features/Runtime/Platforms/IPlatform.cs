using VoxelEngine.Graphics;

namespace VoxelEngine.Core;

public interface IPlatform
{
    GraphicsContext CreateGraphicsContext();

    // IInputModule CreateInputModule();
    // IAudioModule CreateAudioModule();
    // IPhysicsModule CreatePhysicsModule();

    // IFileManager CreateFileManager();
}