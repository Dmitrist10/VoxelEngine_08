using System.Numerics;
using VoxelEngine.Core;

namespace VoxelEngine.Audio;

public interface IAudio
{
    AudioHandle LoadAudioBuffer(AudioData data);
    void UnloadAudioBuffer(AudioHandle handle);

    uint PlaySound(AudioAsset asset, C_AudioSource settings, Vector3 position);
    void UpdateSourcePosition(uint sourceId, Vector3 position);
    void UpdateListener(Vector3 position, Vector3 forward, Vector3 up);
}