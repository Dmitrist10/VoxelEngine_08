using System.Security;

namespace VoxelEngine.IO;

public sealed class FileManager
{
    private readonly string _rootPath;
    private readonly Dictionary<string, IFilesProvider> _providers = new Dictionary<string, IFilesProvider>();

    private bool isRestricedMode = false;
    private DiskFilesProvider _rootProvider;

    public FileManager()
    {
        _rootPath = Environment.CurrentDirectory;
        _providers.Add("root://", _rootProvider = new DiskFilesProvider(_rootPath, "root://"));
        _providers.Add("BuildIn://", new BuiltInFilesProvider("BuildIn://"));
    }

    internal void SetRestrictedMode(bool value)
    {
        isRestricedMode = value;
    }

    public void Mount(string alias, IFilesProvider provider)
    {
        // if (isRestricedMode)
        // throw new SecurityException("FileManager is in restricted mode");

        _providers.Add(alias, provider);
    }

    [MethodImpl(AggressiveInlining)]
    public string ToProvider(string path) => path.Split("://")[0];
    [MethodImpl(AggressiveInlining)]
    public string ToAbsolutePath(string path) => path.Split("://")[1];



}

internal class DiskFilesProvider : IFilesProvider
{
    private readonly string _rootPath;
    private readonly string _alias;
    public DiskFilesProvider(string rootPath, string alias) => (_rootPath, _alias) = (rootPath, alias);

}

public interface IFilesProvider
{
}