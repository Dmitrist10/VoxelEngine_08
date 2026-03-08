using Silk.NET.OpenAL;
using System.Numerics;
using VoxelEngine.Core;
using VoxelEngine.Graphics;

namespace VoxelEngine.Audio;

public sealed unsafe class AudioModule : IAudioModule, IAudio
{
    private ALContext alc = null!;
    private AL al = null!;
    private Device* device;
    private Context* context;

    public void OnInitialize() { }

    public void OnLoad(IWindowSurface window)
    {
        alc = ALContext.GetApi();
        al = AL.GetApi();

        device = alc.OpenDevice("");
        context = alc.CreateContext(device, null);
        alc.MakeContextCurrent(context);

        al.DistanceModel(DistanceModel.InverseDistanceClamped);
        al.SetListenerProperty(ListenerVector3.Position, 0f, 0f, 0f);
        al.SetListenerProperty(ListenerVector3.Velocity, 0f, 0f, 0f);

        float[] orientation = new float[] { 0f, 0f, -1f, 0f, 1f, 0f };
        fixed (float* oriPtr = orientation)
        {
            al.SetListenerProperty(ListenerFloatArray.Orientation, oriPtr);
        }

        ServiceContainer.Register<IAudio>(this);
        Console.WriteLine("[AudioModule] Loaded OpenAL successfully.");
    }

    public AudioHandle LoadAudioBuffer(AudioData data)
    {
        uint buffer = al.GenBuffer();

        BufferFormat alFormat = data.Format switch
        {
            AudioFormat.Mono8 => BufferFormat.Mono8,
            AudioFormat.Mono16 => BufferFormat.Mono16,
            AudioFormat.Stereo8 => BufferFormat.Stereo8,
            AudioFormat.Stereo16 => BufferFormat.Stereo16,
            _ => throw new Exception("Invalid audio format")
        };

        fixed (byte* dataPtr = data.Data)
        {
            al.BufferData(buffer, alFormat, dataPtr, data.Data.Length, data.SampleRate);
        }

        Console.WriteLine($"[AudioModule] Loaded buffer {buffer} - Format: {alFormat}, Rate: {data.SampleRate}");
        return new AudioHandle(new ResourceHandle(buffer, 0));
    }

    public void UnloadAudioBuffer(AudioHandle handle)
    {
        uint bufferId = handle.Handle.Index;
        al.DeleteBuffer(bufferId);
    }

    public uint PlaySound(AudioAsset asset, C_AudioSource settings, Vector3 position)
    {
        uint source = al.GenSource();
        uint bufferId = asset.Handle.Handle.Index;

        al.SetSourceProperty(source, SourceInteger.Buffer, bufferId);

        al.SetSourceProperty(source, SourceFloat.Pitch, settings.Pitch);
        al.SetSourceProperty(source, SourceFloat.Gain, settings.Volume);
        al.SetSourceProperty(source, SourceBoolean.Looping, settings.Looping);

        if (settings.Is3D)
        {
            if (asset.Data.Format == AudioFormat.Stereo8 || asset.Data.Format == AudioFormat.Stereo16)
            {
                Console.WriteLine("[AudioModule] WARNING: 3D Audio requires Mono sound files! Your Stereo file will play in 2D.");
            }

            al.SetSourceProperty(source, SourceBoolean.SourceRelative, false);
            al.SetSourceProperty(source, SourceVector3.Position, position.X, position.Y, position.Z);
            al.SetSourceProperty(source, SourceFloat.ReferenceDistance, settings.ReferenceDistance);
            al.SetSourceProperty(source, SourceFloat.MaxDistance, settings.MaxDistance);
            al.SetSourceProperty(source, SourceFloat.RolloffFactor, settings.RolloffFactor);
        }
        else
        {
            al.SetSourceProperty(source, SourceBoolean.SourceRelative, true);
            al.SetSourceProperty(source, SourceVector3.Position, 0, 0, 0);
        }

        al.SourcePlay(source);

        return source;
    }

    public void UpdateSourcePosition(uint sourceId, Vector3 position)
    {
        al.SetSourceProperty(sourceId, SourceVector3.Position, position.X, position.Y, position.Z);
    }

    public void UpdateListener(Vector3 position, Vector3 forward, Vector3 up)
    {
        al.SetListenerProperty(ListenerVector3.Position, position.X, position.Y, position.Z);

        float[] orientation = new float[] { forward.X, forward.Y, forward.Z, up.X, up.Y, up.Z };
        fixed (float* oriPtr = orientation)
        {
            al.SetListenerProperty(ListenerFloatArray.Orientation, oriPtr);
        }
    }

    public void OnUpdate()
    {
        // Source pool cleanup can go here
    }

    public void Cleanup()
    {
        if (alc != null)
        {
            if (context != null) alc.DestroyContext(context);
            if (device != null) alc.CloseDevice(device);
            alc.Dispose();
        }
        if (al != null) al.Dispose();

        Console.WriteLine("[AudioModule] Cleaned up.");
    }
}