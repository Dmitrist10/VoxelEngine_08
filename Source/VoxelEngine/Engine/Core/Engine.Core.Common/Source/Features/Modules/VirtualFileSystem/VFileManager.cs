using System;
using System.Collections.Generic;
using System.IO;

namespace VoxelEngine.Core.VirtualFileSystem;

public class VFileManager
{
    private Dictionary<string, IVirtualFileProvider> _mounts = new Dictionary<string, IVirtualFileProvider>(StringComparer.OrdinalIgnoreCase);

    public void Mount(string mountPoint, IVirtualFileProvider provider)
    {
        _mounts[mountPoint] = provider;
    }

    public void Unmount(string mountPoint)
    {
        _mounts.Remove(mountPoint);
    }

    private (IVirtualFileProvider provider, string relativePath) ResolvePath(string virtualPath)
    {
        // Example: vfs://builtin/Primitives/Cube -> scheme: vfs, host: builtin, path: /Primitives/Cube
        if (!Uri.TryCreate(virtualPath, UriKind.Absolute, out Uri uri))
        {
            // Fallback for paths without scheme, assume physical disk provider if mounted to root or return null
            throw new ArgumentException($"Invalid virtual path format: {virtualPath}");
        }

        string mountPoint = uri.Host; // e.g. "builtin" or "disk"
        string localPath = uri.AbsolutePath.TrimStart('/'); // e.g. "Primitives/Cube"

        if (_mounts.TryGetValue(mountPoint, out IVirtualFileProvider? provider))
        {
            return (provider, localPath);
        }

        throw new FileNotFoundException($"No virtual file provider mounted at '{mountPoint}' for path '{virtualPath}'");
    }

    public bool Exists(string virtualPath)
    {
        try
        {
            var res = ResolvePath(virtualPath);
            return res.provider.Exists(res.relativePath);
        }
        catch
        {
            return false;
        }
    }

    public Stream OpenRead(string virtualPath)
    {
        var res = ResolvePath(virtualPath);
        return res.provider.OpenRead(res.relativePath);
    }

    public string GetAbsolutePath(string virtualPath)
    {
        var res = ResolvePath(virtualPath);
        return res.provider.GetAbsolutePath(res.relativePath);
    }
}
