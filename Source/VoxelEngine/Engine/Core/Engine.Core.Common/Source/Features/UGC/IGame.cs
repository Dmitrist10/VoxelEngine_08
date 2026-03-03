namespace VoxelEngine.Core.UGC;

public interface IGame
{
    string Name { get; }

    void OnInitialize();
    void OnUpdate();
    void OnShutdown();
}
