using System.Security.Principal;
using Arch.Core;
using VoxelEngine.Core.Assets;
using VoxelEngine.Diagnostics;
using VoxelEngine.Graphics;

namespace VoxelEngine.Core.Runtime;

public sealed class Engine
{
    private readonly IPlatform platform;
    private readonly IRuntimeContext runtimeContext;

    private Heartbeat heartbeat;
    private GraphicsContext? graphicsContext;

    public Engine(IPlatform platform, IRuntimeContext runtimeContext)
    {
        this.platform = platform;
        this.runtimeContext = runtimeContext;
        heartbeat = new Heartbeat();
    }

    public void Run()
    {
        Logger.Line();
        Logger.Info("Starting engine runtime...");

        PreInit();
        Init();
        PostInit();

        StartRuntime();

        runtimeContext.Cleanup();

        Logger.Info("Application terminated.");
        Logger.Line();
    }


    private void PreInit()
    {
        graphicsContext = platform.CreateGraphicsContext();
        ServiceContainer.Register<GraphicsContext>(graphicsContext);
    }

    private void Init()
    {
        graphicsContext!.Window.Initialize();


        // Initialize Arch ECS SharedJobScheduler for parallel queries
        var archJobConfig = new Schedulers.JobScheduler.Config
        {
            ThreadPrefixName = "VoxelEngine_Arch",
            ThreadCount = 0, // Auto-detect core count
            MaxExpectedConcurrentJobs = 256,
            StrictAllocationMode = false
        };
        World.SharedJobScheduler = new Schedulers.JobScheduler(archJobConfig);

        AssetsManager assetsManager = new AssetsManager();
        // assetsManager.RegisterLoader<ShaderData>(new ShaderLoader());
        // assetsManager.RegisterLoader<STDMeshData>(new MeshLoader());
        // assetsManager.RegisterLoader<TextureData>(new TextureLoader());
        ServiceContainer.Register<AssetsManager>(assetsManager);
    }
    private void PostInit()
    {
        Logger.Info("Post init...");

        runtimeContext.Init();
    }


    private void StartRuntime()
    {
        heartbeat.Start(runtimeContext, graphicsContext!.Window);
    }

    internal void Stop()
    {
        heartbeat.Stop();
    }


}