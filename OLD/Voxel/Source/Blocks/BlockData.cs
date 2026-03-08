namespace VoxelEngine.Packages.Voxel;

public record BlockData
{
    public string Name { get; init; } = string.Empty;
    public string Id { get; init; } = string.Empty;
    public int RuntimeId { get; init; } = 0;
    public string TexturePath { get; init; } = string.Empty;

    public BlockData(string name, string id, int runtimeId, string texturePath)
    {
        Name = name;
        Id = id;
        RuntimeId = runtimeId;
        TexturePath = texturePath;
    }

}

public readonly struct BlockDescription
{
    public string Name { get; init; } = string.Empty;
    public string Id { get; init; } = string.Empty;
    public string TexturePath { get; init; } = string.Empty;

    public BlockDescription(string name, string id, string texturePath)
    {
        Name = name;
        Id = id;
        TexturePath = texturePath;
    }
}