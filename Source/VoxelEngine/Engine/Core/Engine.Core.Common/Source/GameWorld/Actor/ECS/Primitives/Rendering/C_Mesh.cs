namespace VoxelEngine.Core;

public record struct C_Mesh : IComponent
{
    public MeshAsset Mesh;
    public Material Material;

    public C_Mesh(MeshAsset mesh, Material material)
    {
        Mesh = mesh;
        Material = material;
    }
}

// public struct C_Mesh : IComponent
// {
//     private Guid _meshID;
//     private MaterialAsset _material;

//     public Guid MeshID
//     {
//         [MethodImpl(AggressiveInlining)] get => _meshID;
//         [MethodImpl(AggressiveInlining)]
//         set
//         {
//             if (_meshID != value)
//             {
//                 _meshID = value;
//                 GPUMesh = null; // Invalidate GPU Mesh Cache
//             }
//         }
//     }

//     public MaterialAsset Material
//     {
//         [MethodImpl(AggressiveInlining)] get => _material;
//         [MethodImpl(AggressiveInlining)]
//         set
//         {
//             _material = value;
//             GPUShader = null; // Invalidate GPU Shader Cache
//         }
//     }

//     public GPUMesh? GPUMesh;
//     public GPUShader? GPUShader;

//     public C_Mesh(Guid meshID, MaterialAsset material)
//     {
//         _meshID = meshID;
//         _material = material;

//         GPUMesh = null;
//         GPUShader = null;
//     }
// }

// /// <summary>
// /// A zero-allocation readonly struct proxy that makes it easy to modify C_Mesh 
// /// without using the 'ref' keyword.
// /// </summary>
// public readonly struct MeshProxy
// {
//     private readonly Actor _actor;
//     public MeshProxy(Actor actor) => _actor = actor;

//     public bool IsValid => _actor.HasComponent<C_Mesh>();

//     public Guid MeshID
//     {
//         [MethodImpl(AggressiveInlining)] get => _actor.GetComponent<C_Mesh>().MeshID;
//         [MethodImpl(AggressiveInlining)] set => _actor.GetComponent<C_Mesh>().MeshID = value;
//     }

//     public MaterialAsset Material
//     {
//         [MethodImpl(AggressiveInlining)] get => _actor.GetComponent<C_Mesh>().Material;
//         [MethodImpl(AggressiveInlining)] set => _actor.GetComponent<C_Mesh>().Material = value;
//     }

//     // Direct access to runtime cache if needed
//     public GPUMesh? GPUMesh
//     {
//         [MethodImpl(AggressiveInlining)] get => _actor.GetComponent<C_Mesh>().GPUMesh;
//     }

//     public GPUShader? GPUShader
//     {
//         [MethodImpl(AggressiveInlining)] get => _actor.GetComponent<C_Mesh>().GPUShader;
//     }
// }