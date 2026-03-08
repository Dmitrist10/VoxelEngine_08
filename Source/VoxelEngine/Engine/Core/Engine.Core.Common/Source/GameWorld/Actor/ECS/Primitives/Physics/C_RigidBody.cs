using System.Numerics;
using VoxelEngine.Core;

namespace VoxelEngine.Physics;

public record struct C_RigidBody
{
    public float Mass;
    public float Restitution;
    public float Friction;

    public Vector3 velocity;

    public ResourceHandle BodyHandle;

    public C_RigidBody(float mass, float restitution, float friction)
    {
        Mass = mass;
        Restitution = restitution;
        Friction = friction;
    }
}

public record struct C_BoxCollider
{
    public Vector3 center;
    public Vector3 size;

    public ResourceHandle ColliderHandle;

    public C_BoxCollider(Vector3 center, Vector3 size)
    {
        this.center = center;
        this.size = size;
    }
}

public record struct C_MeshCollider
{
    public Vector3 center;
    public MeshAsset? mesh;

    public ResourceHandle ColliderHandle;

    public C_MeshCollider(Vector3 center, MeshAsset mesh)
    {
        this.center = center;
        this.mesh = mesh;
    }
}