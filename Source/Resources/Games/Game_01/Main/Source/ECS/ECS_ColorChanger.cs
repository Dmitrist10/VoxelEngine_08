using Arch.Core;
using VoxelEngine.Core;

namespace CustomGame;


public record struct C_ColorChanger : IComponent
{
}

public sealed class EP_ColorChanger : EntityProcessor, IFixedUpdatable
{

    private QueryDescription _query;

    public override void OnInitialize()
    {
        _query = new QueryDescription().WithAll<C_ColorChanger, C_Mesh>();
    }

    public void OnFixedUpdate()
    {
        var r = (float)(EMath.Sin(((float)Time.TotalTime) * 2f) * 0.5f + 0.5f);
        var g = (float)(EMath.Sin(((float)Time.TotalTime) * 3f) * 0.5f + 0.5f);
        var b = (float)(EMath.Sin(((float)Time.TotalTime) * 4f) * 0.5f + 0.5f);

        Color c = new Color(r, g, b, 1.0f);

        world.Query(_query, (ref C_ColorChanger colorChanger, ref C_Mesh mesh) =>
        {
            var mat = (PBRMaterial)mesh.Material;
            mat.Properties.Color = c;
            mat.ApplyChanges();
        });
    }

}