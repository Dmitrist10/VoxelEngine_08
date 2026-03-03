namespace VoxelEngine.Core;

public interface IBehavior
{
    void OnAwake();
    void OnStart();
    void OnDestroy();

    void Invalidate();
}