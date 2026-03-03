using VoxelEngine.Core;
using System.Numerics;

namespace VoxelEngine.Client.Rendering;

public readonly record struct RenderCommand(
    MeshAsset Mesh,
    Material Material,
    Matrix4x4 Transform
);