namespace VoxelEngine.Core;

public sealed class CamerasRegistries
{
    List<CameraData> cameras = new List<CameraData>();
    public IReadOnlyCollection<CameraData> Cameras => cameras;

    public void Sort()
    {
        cameras.Sort((a, b) => a.priority - b.priority);
    }

    public void Add(CameraData c)
    {
        cameras.Add(c);
    }

    public void Clear()
    {
        cameras.Clear();
    }

}
