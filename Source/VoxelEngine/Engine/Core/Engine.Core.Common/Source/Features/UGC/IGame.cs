namespace VoxelEngine.Core.UGC;

public interface IGame
{
    string Name { get; }

    void SetUp(UniverseManager universeManager);

    void StartMatch();

    void OnInitialize();
    void OnUpdate();
    void OnShutdown();
}
