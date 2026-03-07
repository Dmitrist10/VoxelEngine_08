using System.Collections.Generic;
using System.Numerics;
using VoxelEngine.Core;
using VoxelEngine.Diagnostics;
using VoxelEngine.Graphics;

namespace CustomGame;

public static class GameSetup
{
    public static void SetUp(Scene scene)
    {
        Logger.Debug("GameSetup.SetUp was called!");
        var manager = ServiceContainer.Get<AssetsManager>()!;

        MeshAsset triangleAsset = BuildInAssets.GetBuiltInMesh("Triangle");
        MeshAsset cubeAsset = BuildInAssets.GetBuiltInMesh("Cube");

        string treePath = @"C:\Users\Dmitrist10\Desktop\VoxelGames\Source\Project_08_VE\Source\Resources\Games\Game_01\Assets\Models\Tree\Tree.obj";
        STDMeshData treeData = manager.Load<STDMeshData>($"vfs://disk/{treePath.Replace('\\', '/')}").Get();
        MeshAsset treeMesh = new MeshAsset(ServiceContainer.Get<GraphicsContext>()!.Device.Factory.CreateMesh<STDVertex>(treeData), treeData.VertexCount, treeData.IndexCount);

        PBRMaterial mat1 = BuildInAssets.GetPBRMaterial();
        TextureMaterial treeMat = BuildInAssets.GetTreeTextureMaterial();

        Actor cubeActor = scene.CreateActor();
        cubeActor.AddComponent(new C_Mesh(cubeAsset, mat1));
        cubeActor.AddComponent<C_ColorChanger>();
        cubeActor.AddComponent(new C_Rotator(2));
        cubeActor.Position = new Vector3(-2, 0, 0);

        Actor triangleActor = scene.CreateActor();
        triangleActor.AddComponent(new C_Mesh(triangleAsset, mat1));
        triangleActor.AddComponent<C_ColorChanger>();
        triangleActor.Position = new Vector3(2, 0, 0);

        Actor treeActor = scene.CreateActor();
        treeActor.AddComponent(new C_Mesh(treeMesh, treeMat));
        treeActor.Position = new Vector3(0, -4, 5);

        // Camera
        Actor cameraActor = scene.CreateActor();
        C_Camera camera = new C_Camera(CameraProjectionType.Perspective);
        cameraActor.AddComponent(camera);
        cameraActor.Position = new Vector3(0, 0, -10);

        scene.AddProcessor(new EP_ColorChanger());
        scene.AddProcessor(new EP_Rotator());

        // for (int i = -50; i < 50; i++)
        // {
        //     Actor a = scene.CreateActor();
        //     a.AddComponent(new C_Mesh(triangleAsset, new PBRMaterial(new PBRMaterialProperties() { Color = Color.Orange }) { Pipeline = pipeline }));
        //     a.AddComponent<C_ColorChanger>();
        //     a.Position = new Vector3(i * 0.75f, i * 0.1f, i * 0.5f);
        // }
    }

}
