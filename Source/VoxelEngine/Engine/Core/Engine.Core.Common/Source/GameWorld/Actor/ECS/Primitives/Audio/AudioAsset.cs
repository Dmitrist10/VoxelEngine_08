using VoxelEngine.Core;

namespace VoxelEngine.Audio;

public enum AudioFormat
{
    Mono8,
    Mono16,
    Stereo8,
    Stereo16
}

public record struct AudioAsset : IAsset
{
    public AudioHandle Handle;
    public AudioData Data;

    public AudioAsset(AudioHandle handle, AudioData data)
    {
        Handle = handle;
        Data = data;
    }
}

public record AudioData : IAssetData
{
    public byte[] Data;
    public int Channels;
    public int SampleRate;
    public int BitsPerSample;
    public AudioFormat Format;

    public AudioData(byte[] data, int channels, int sampleRate, int bitsPerSample, AudioFormat format)
    {
        Data = data;
        Channels = channels;
        SampleRate = sampleRate;
        BitsPerSample = bitsPerSample;
        Format = format;
    }
}