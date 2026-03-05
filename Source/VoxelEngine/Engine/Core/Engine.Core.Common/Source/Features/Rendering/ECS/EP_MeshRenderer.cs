using Arch.Core;
using VoxelEngine.Core;
using VoxelEngine.Diagnostics;

namespace VoxelEngine.Core.Rendering;

public sealed class EP_MeshRenderer : EntityProcessor, IRenderable
{

    private QueryDescription _query;
    private RenderManager _renderManager;

    public EP_MeshRenderer()
    {
        _renderManager = ServiceContainer.Get<RenderManager>()!;
    }

    public override void OnInitialize()
    {
        _query = new QueryDescription().WithAll<C_Mesh, C_WorldTransformMatrix>();
    }

    public void OnRender()
    {
        // MeshRenderingQuery meshRenderingQuery = new(_renderManager);
        // world.InlineQuery(_query, ref meshRenderingQuery);

        world.Query(_query, (ref C_Mesh mesh, ref C_WorldTransformMatrix transform) =>
        {
            _renderManager.Submit(new RenderCommand(
                mesh.Mesh,
                mesh.Material,
                transform.WorldMatrix
            ));
        });

    }

}

// public struct MeshRenderingQuery : IForEach<C_Mesh, C_WorldTransformMatrix>
// {
//     public RenderManager _renderManager;

//     public MeshRenderingQuery(RenderManager renderManager)
//     {
//         _renderManager = renderManager;
//     }

//     [MethodImpl(MethodImplOptions.AggressiveInlining)]
//     public void Update(ref C_Mesh mesh, ref C_WorldTransformMatrix transform)
//     {
//         _renderManager.Submit(new RenderCommand(
//             mesh.Mesh,
//             mesh.IndexCount,
//             mesh.Pipeline,
//             mesh.MaterialUniforms,
//             transform.WorldMatrix
//         ));
//     }

// }
