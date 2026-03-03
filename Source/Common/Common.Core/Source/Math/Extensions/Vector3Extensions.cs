using System.Numerics;

namespace VoxelEngine.Core;

public static class Vector3Extensions
{
    // public static Vector3 MoveTowards(this Vector3 current, Vector3 target, float delta)
    // {

    // }

    extension(Vector3 from)
    {
        public Int3 ToInt3()
        {
            return new Int3((int)from.X, (int)from.Y, (int)from.Z);
        }

    }

}

