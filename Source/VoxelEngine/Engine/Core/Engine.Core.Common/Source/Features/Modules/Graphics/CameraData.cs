using System.Numerics;

namespace VoxelEngine.Core;

public struct CameraData
{
    public Matrix4x4 view;
    public Matrix4x4 projection;
    public int priority;

    public CameraData(Matrix4x4 view, Matrix4x4 projection, int priority)
    {
        this.view = view;
        this.projection = projection;
        this.priority = priority;
    }

    /// <summary>
    /// Converts a 2D screen position into a 3D world-space ray (Origin, Direction).
    /// </summary>
    public (Vector3 Origin, Vector3 Direction) ScreenPointToRay(Vector2 screenPosition, Vector2 windowSize)
    {
        float ndcX = (2.0f * screenPosition.X) / windowSize.X - 1.0f;
        float ndcY = 1.0f - (2.0f * screenPosition.Y) / windowSize.Y; // Flipped Y for NDC

        if (Matrix4x4.Invert(view * projection, out Matrix4x4 invViewProj))
        {
            Vector4 nearPoint = new Vector4(ndcX, ndcY, -1f, 1f);
            nearPoint = Vector4.Transform(nearPoint, invViewProj);
            if (nearPoint.W != 0) nearPoint /= nearPoint.W;

            Vector4 farPoint = new Vector4(ndcX, ndcY, 1f, 1f);
            farPoint = Vector4.Transform(farPoint, invViewProj);
            if (farPoint.W != 0) farPoint /= farPoint.W;

            Vector3 rayStart = new Vector3(nearPoint.X, nearPoint.Y, nearPoint.Z);
            Vector3 rayDir = Vector3.Normalize(new Vector3(farPoint.X, farPoint.Y, farPoint.Z) - rayStart);

            return (rayStart, rayDir);
        }

        return (Vector3.Zero, Vector3.UnitZ);
    }
}
