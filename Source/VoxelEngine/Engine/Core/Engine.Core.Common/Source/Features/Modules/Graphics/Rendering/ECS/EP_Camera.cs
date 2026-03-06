using Arch.Core;
using VoxelEngine.Core;
using VoxelEngine.Graphics;

namespace VoxelEngine.Graphics.Rendering;

public sealed class EP_Camera : EntityProcessor, IRenderable
{
    private QueryDescription query;
    private CamerasRegistries camerasRegistries;
    private IWindowSurface window;

    public EP_Camera()
    {
        camerasRegistries = ServiceContainer.Get<CamerasRegistries>()!;
        window = ServiceContainer.Get<GraphicsContext>()!.Window;
    }


    public override void OnInitialize()
    {
        query = new QueryDescription().WithAll<C_Transform, C_Camera>();
    }

    public void OnRender()
    {
        float aspectRatio = window.Size.X / window.Size.Y;

        world.Query(in query, (ref C_Transform transform, ref C_Camera camera) =>
        {
            camera.UpdateView(transform.WorldPosition, transform.WorldPosition + transform.Forward, transform.Up);
            camera.UpdateAspectRatio(aspectRatio); // Auto update projection
            camerasRegistries.Add(new CameraData(camera.View, camera.Projection, camera.Priority));
        });
    }
}
