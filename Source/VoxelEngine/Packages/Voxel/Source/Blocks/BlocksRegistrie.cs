using VoxelEngine.Core;

namespace VoxelEngine.Packages.Voxel;

public sealed class BlocksRegistrie
{

    private readonly List<BlockData> _registationBlocks = new();
    private BlockData[] _allBlocks = [];

    public void Register(BlockDescription block)
    {
        BlockData data = new(block.Name, block.Id, _registationBlocks.Count + 1, block.TexturePath);
        _registationBlocks.Add(data);
    }

    public void Build()
    {
        _allBlocks = new BlockData[_registationBlocks.Count + 1];

        var Air = new BlockData("Air", "STD:Air", 0, "");
        _allBlocks[0] = new BlockData("Air", "STD:Air", 0, "");
        _registationBlocks.Insert(0, Air);

        for (int i = 1; i < _registationBlocks.Count; i++)
        {
            _allBlocks[i] = _registationBlocks[i];
        }
        _registationBlocks.Clear();

        string[] filePaths = new string[_allBlocks.Length];

        TextureArrayBuilder.Build();
    }

    public BlockData GetBlock(int id)
    {
        return _allBlocks[id];
    }


}
