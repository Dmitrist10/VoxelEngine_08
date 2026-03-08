using System.Numerics;
using VoxelEngine.Core;

namespace VoxelEngine.Audio;

public static class AudioManager
{
    private static IAudio? _audio;
    private static IAudio Audio => _audio ??= ServiceContainer.Get<IAudio>()
                        ?? throw new InvalidOperationException(
                            "Audio module has not been loaded. " +
                            "Ensure IAudioModule.OnLoad() is called before accessing Input.");

    public static AudioHandle LoadAudioBuffer(AudioData data) => Audio.LoadAudioBuffer(data);
    public static void UnloadAudioBuffer(AudioHandle handle) => Audio.UnloadAudioBuffer(handle);

    public static uint PlaySound(AudioAsset buffer, C_AudioSource settings, Vector3 position)
        => Audio.PlaySound(buffer, settings, position);

    public static void UpdateSourcePosition(uint sourceId, Vector3 position)
        => Audio.UpdateSourcePosition(sourceId, position);

    public static void UpdateListener(Vector3 position, Vector3 forward, Vector3 up)
        => Audio.UpdateListener(position, forward, up);
}