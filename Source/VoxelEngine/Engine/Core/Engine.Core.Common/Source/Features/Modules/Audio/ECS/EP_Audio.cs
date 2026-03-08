using Arch.Core;
using VoxelEngine.Core;
using VoxelEngine.Audio;
using VoxelEngine.Diagnostics;

namespace VoxelEngine.Audio.ECS;

public sealed class EP_Audio : EntityProcessor, IUpdatable
{
    private QueryDescription listenerQuery;
    private QueryDescription sourceQuery;

    public override void OnInitialize()
    {
        listenerQuery = new QueryDescription().WithAll<C_Transform, C_AudioListener>();
        sourceQuery = new QueryDescription().WithAll<C_Transform, C_AudioSource>();
    }

    public void OnUpdate()
    {
        // 1. Update the listener position
        world.Query(in listenerQuery, (ref C_Transform transform, ref C_AudioListener listener) =>
        {
            if (listener.IsActive)
            {
                AudioManager.UpdateListener(transform.WorldPosition, transform.Forward, transform.Up);
            }
        });

        // 2. Play Audio Sources
        world.Query(in sourceQuery, (ref C_Transform transform, ref C_AudioSource source) =>
        {
            if (source.PlayOnAwake && !source.IsPlaying)
            {
                if (source.Audio.Handle.Handle.IsValid)
                {
                    source.SourceId = AudioManager.PlaySound(source.Audio, source, transform.WorldPosition);
                    source.IsPlaying = true;
                }
            }
            else if (source.IsPlaying && source.Is3D && source.SourceId != 0)
            {
                AudioManager.UpdateSourcePosition(source.SourceId, transform.WorldPosition);
            }
        });
    }
}
