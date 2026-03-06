using System.Numerics;
using Arch.Core;
using VoxelEngine.Core;

namespace CustomGame;

internal record struct C_Rotator
{
    public float Speed = 1f;

    public C_Rotator(float speed)
    {
        Speed = speed;
    }
}

internal class EP_Rotator : EntityProcessor, IUpdatable
{
    private QueryDescription _query;

    public override void OnInitialize()
    {
        _query = new QueryDescription().WithAll<C_Rotator, C_Transform>();
    }

    public void OnUpdate()
    {
        world.Query(_query, (ref C_Rotator rotator, ref C_Transform transform) =>
        {
            transform.LocalRotation *= Quaternion.CreateFromAxisAngle(Vector3.UnitY, rotator.Speed * Time.DeltaTime);
            transform.MarkDirty();
        });
    }
}
