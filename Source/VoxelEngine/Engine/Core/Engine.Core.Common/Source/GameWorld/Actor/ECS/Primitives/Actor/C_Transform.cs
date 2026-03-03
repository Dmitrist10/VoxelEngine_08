using System.Numerics;

// namespace Engine.VoxelEngine.Core.Architecture.Universe.Actors;
namespace VoxelEngine.Core;

public record struct C_Transform : IComponent
{
    // Local transform (relative to parent)
    public Vector3 LocalPosition;
    public Quaternion LocalRotation;
    public Vector3 LocalScale;

    // World transform 
    public Vector3 WorldPosition;
    public Quaternion WorldRotation;
    public Vector3 WorldScale;

    // Flags
    public bool IsDirty;

    public C_Transform()
    {
        LocalPosition = Vector3.Zero;
        LocalRotation = Quaternion.Identity;
        LocalScale = Vector3.One;

        WorldPosition = Vector3.Zero;
        WorldRotation = Quaternion.Identity;
        WorldScale = Vector3.One;

        IsDirty = true;
    }

    public Vector3 Forward => Vector3.Transform(Vector3.UnitZ, WorldRotation);
    public Vector3 Right => Vector3.Transform(Vector3.UnitX, WorldRotation);
    public Vector3 Up => Vector3.Transform(Vector3.UnitY, WorldRotation);

    [MethodImpl(AggressiveInlining)]
    public void MarkDirty() => IsDirty = true;
}

public record struct C_WorldTransformMatrix : IComponent
{
    public Matrix4x4 WorldMatrix;

    public C_WorldTransformMatrix()
    {
        WorldMatrix = Matrix4x4.Identity;
    }
}
