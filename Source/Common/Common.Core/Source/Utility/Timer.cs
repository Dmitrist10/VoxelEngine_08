namespace VoxelEngine.Core;

public sealed class Timer
{
    public float time;
    public float duration;
    public bool isDone { get; private set; }

    public Timer(float duration)
    {
        this.duration = duration;
    }

    public void Reset(float duration)
    {
        this.duration = duration;
        Reset();
    }
    public void Reset()
    {
        time = 0;
        isDone = false;
    }

    public void Update(float dt)
    {
        time += dt;
        if (time >= duration)
        {
            isDone = true;
        }
    }


}