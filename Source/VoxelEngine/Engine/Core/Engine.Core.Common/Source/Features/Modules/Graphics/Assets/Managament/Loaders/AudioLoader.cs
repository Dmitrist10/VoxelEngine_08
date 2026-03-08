using Assimp;
using System.Numerics;
using NAudio.Wave;
using NVorbis;
using VoxelEngine.Audio;

namespace VoxelEngine.Core.Assets;

public class AudioLoader : IAssetLoader<AudioData>
{
    public AudioData Load(string path)
    {
        string extension = Path.GetExtension(path).ToLowerInvariant();

        if (extension == ".wav")
        {
            using var reader = new WaveFileReader(path);
            var format = reader.WaveFormat;

            byte[] buffer = new byte[reader.Length];
            reader.Read(buffer, 0, buffer.Length);

            AudioFormat alFormat = GetAudioFormat(format.Channels, format.BitsPerSample);

            return new AudioData(buffer, format.Channels, format.SampleRate, format.BitsPerSample, alFormat);
        }
        else if (extension == ".ogg")
        {
            using var vorbis = new VorbisReader(path);

            int channels = vorbis.Channels;
            int sampleRate = vorbis.SampleRate;

            int bufferSize = (int)(vorbis.TotalSamples * channels);
            float[] floatBuffer = new float[bufferSize];
            vorbis.ReadSamples(floatBuffer, 0, bufferSize);

            // Convert float samples to 16-bit PCM (shorts) to byte array
            byte[] byteBuffer = new byte[bufferSize * 2];
            for (int i = 0; i < floatBuffer.Length; i++)
            {
                short val = (short)Math.Clamp((int)(floatBuffer[i] * 32767.0f), short.MinValue, short.MaxValue);
                byteBuffer[i * 2] = (byte)(val & 0xFF);
                byteBuffer[i * 2 + 1] = (byte)((val >> 8) & 0xFF);
            }

            AudioFormat alFormat = GetAudioFormat(channels, 16);
            return new AudioData(byteBuffer, channels, sampleRate, 16, alFormat);
        }
        else if (extension == ".mp3")
        {
            using var reader = new Mp3FileReader(path);
            var format = reader.WaveFormat;

            byte[] buffer = new byte[reader.Length];
            reader.Read(buffer, 0, buffer.Length);

            AudioFormat alFormat = GetAudioFormat(format.Channels, format.BitsPerSample);

            return new AudioData(buffer, format.Channels, format.SampleRate, format.BitsPerSample, alFormat);
        }
        else
        {
            throw new NotSupportedException($"Audio format '{extension}' is not supported! Please use .wav, .ogg, or .mp3.");
        }
    }

    private AudioFormat GetAudioFormat(int channels, int bitsPerSample)
    {
        if (channels == 1 && bitsPerSample == 8) return AudioFormat.Mono8;
        if (channels == 1 && bitsPerSample == 16) return AudioFormat.Mono16;
        if (channels == 2 && bitsPerSample == 8) return AudioFormat.Stereo8;
        if (channels == 2 && bitsPerSample == 16) return AudioFormat.Stereo16;
        throw new NotSupportedException($"Unsupported audio format: Channels: {channels}, Bits: {bitsPerSample}");
    }
}