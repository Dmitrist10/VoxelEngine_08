namespace VoxelEngine.Core;

public readonly record struct ResourceHandle(uint Index, uint Generation)
{
    public bool IsValid => Index != 0 || Generation != 0;
}
