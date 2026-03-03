using System.Numerics;

namespace VoxelEngine.Core;

public enum CameraProjectionType
{
    Perspective,
    Orthographic
}

public struct C_Camera : IComponent
{
    public Matrix4x4 View { get; private set; }
    public Matrix4x4 Projection { get; private set; }
    public bool IsMainCamera { get; internal set; }
    public int Priority { get; internal set; }

    public CameraProjectionType CameraType;

    public float OrthographicSize;
    public float FieldOfView;

    public float AspectRatio;
    public float NearPlane;
    public float FarPlane;

    public C_Camera(CameraProjectionType type = CameraProjectionType.Perspective, float fieldOfView = 0.8f, float aspectRatio = 16f / 9f,
        float nearPlane = 0.1f, float farPlane = 1000f, float orthographicSize = 8f, bool isMainCamera = false,
        int Priority = 0)
    {
        CameraType = type;
        FieldOfView = fieldOfView;
        AspectRatio = aspectRatio;
        NearPlane = nearPlane;
        FarPlane = farPlane;
        OrthographicSize = orthographicSize;
        IsMainCamera = isMainCamera;
        this.Priority = Priority;
        UpdateProjection();
    }

    public void UpdateProjection()
    {
        switch (CameraType)
        {
            case CameraProjectionType.Perspective:
                if (FieldOfView <= 0) FieldOfView = 0.75f;
                Projection = Matrix4x4.CreatePerspectiveFieldOfView(FieldOfView, AspectRatio, NearPlane, FarPlane);
                break;
            case CameraProjectionType.Orthographic:
                Projection = Matrix4x4.CreateOrthographic(OrthographicSize * AspectRatio, OrthographicSize, NearPlane,
                    FarPlane);
                break;
        }
    }

    public void UpdateView(Vector3 cameraPos, Vector3 cameraTarget, Vector3 cameraUp)
    {
        // View = Matrix4x4.CreateLookAt(transform.Position, transform.Position + Transform.Forward, Transform.Up);
        View = Matrix4x4.CreateLookAt(cameraPos, cameraTarget, cameraUp);
    }

    public void UpdateAspectRatio(float aspectRatio)
    {
        AspectRatio = aspectRatio;
        UpdateProjection();
    }
}