using System.IO;

namespace VoxelEngine.Core.VirtualFileSystem;

public class PhysicalFileProvider : IVirtualFileProvider
{
    private string _rootPath;

    public PhysicalFileProvider(string rootPath)
    {
        _rootPath = rootPath;
    }

    public bool Exists(string path)
    {
        return File.Exists(Path.Combine(_rootPath, path));
    }

    public Stream OpenRead(string path)
    {
        return File.OpenRead(Path.Combine(_rootPath, path));
    }

    public string GetAbsolutePath(string path)
    {
        return Path.Combine(_rootPath, path);
    }
}
