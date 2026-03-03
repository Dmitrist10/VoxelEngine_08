namespace VoxelEngine.Core.Runtime;

public sealed class EngineBuilder
{
    private IPlatform? platform;
    private IRuntimeContext? runtimeContext;

    public EngineBuilder()
    {

    }

    public EngineBuilder WithPlatform(IPlatform platform)
    {
        this.platform = platform;
        return this;
    }

    public EngineBuilder WithRuntimeContext(IRuntimeContext runtimeContext)
    {
        this.runtimeContext = runtimeContext;
        return this;
    }

    public Engine Build()
    {
        if (platform == null || runtimeContext == null)
            throw new InvalidOperationException("Setup is not Completed!");

        return new Engine(platform, runtimeContext);
    }

}
