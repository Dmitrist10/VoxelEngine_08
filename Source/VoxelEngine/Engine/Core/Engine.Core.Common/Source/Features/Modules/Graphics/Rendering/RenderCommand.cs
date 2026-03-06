using VoxelEngine.Core;
using System.Numerics;

namespace VoxelEngine.Graphics.Rendering;

public readonly record struct RenderCommand(
    MeshAsset Mesh,
    Material Material,
    Matrix4x4 Transform
);
