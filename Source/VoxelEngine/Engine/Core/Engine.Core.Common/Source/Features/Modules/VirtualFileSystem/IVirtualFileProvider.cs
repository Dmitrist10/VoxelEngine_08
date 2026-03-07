using System.IO;

namespace VoxelEngine.Core.VirtualFileSystem;

public interface IVirtualFileProvider
{
    bool Exists(string path);
    Stream OpenRead(string path);
    string GetAbsolutePath(string path); // Useful for error reporting or meta files
}
