namespace VoxelEngine.Audio;

public record struct C_AudioSource
{
    public AudioAsset Audio;
    public float Volume = 1.0f;
    public float Pitch = 1.0f;
    public bool Looping = false;

    // 3D Audio Settings
    public bool Is3D = true;
    public float ReferenceDistance = 2.0f; // Distance where falloff starts
    public float MaxDistance = 15.0f;      // Max distance before fully silent
    public float RolloffFactor = 1.0f;     // Curve intensity

    public bool PlayOnAwake = true;
    public bool IsPlaying = false;
    public uint SourceId = 0;

    public C_AudioSource() { }
}
