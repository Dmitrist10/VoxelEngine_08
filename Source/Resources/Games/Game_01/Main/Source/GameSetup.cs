using System.Collections.Generic;
using System.Numerics;
using VoxelEngine.Audio;
using VoxelEngine.Core;
using VoxelEngine.Diagnostics;
using VoxelEngine.Graphics;
using VoxelEngine.Packages.Voxel;

namespace CustomGame;

public static class GameSetup
{
    public static void SetUp(Scene scene)
    {
        Logger.Debug("GameSetup.SetUp was called!");
        var assetsManager = ServiceContainer.Get<AssetsManager>()!;

        MeshAsset triangleAsset = BuildInAssets.GetBuiltInMesh("Triangle");
        MeshAsset cubeAsset = BuildInAssets.GetBuiltInMesh("Cube");

        string treePath = @"C:\Users\Dmitrist10\Desktop\VoxelGames\Source\Project_08_VE\Source\Resources\Games\Game_01\Assets\Models\Tree\Tree.obj";
        MeshAsset treeMesh = assetsManager.GetMesh(treePath);

        PBRMaterial mat1 = BuildInAssets.GetPBRMaterial();
        // PBRMaterial treeMat = BuildInAssets.GetPBRMaterial();
        TextureMaterial treeMat = BuildInAssets.GetTreeTextureMaterial();

        AudioAsset audioAsset = assetsManager.GetAudio(@"C:\Users\Dmitrist10\Desktop\VoxelGames\Source\Project_08_VE\Source\Resources\Games\Game_01\Assets\Audio\Runes.wav");

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
        B_CameraController cameraController = new();
        cameraActor.AddComponent(camera);
        cameraActor.AddComponent<C_AudioListener>(new C_AudioListener());
        cameraActor.AddBehavior(cameraController);
        cameraActor.Position = new Vector3(0, 15, -10);
        // cameraActor.Rotation = Quaternion.CreateFromYawPitchRoll(0, 50, 0);

        // Actor audioActor = scene.CreateActor();
        // audioActor.AddComponent(new C_Mesh(cubeAsset, mat1));
        // audioActor.AddComponent(new C_AudioSource() { Audio = audioAsset, Looping = true });
        // audioActor.Position = new Vector3(-5, 10, -5);

        // Actor audio2Actor = scene.CreateActor();
        // audio2Actor.AddComponent(new C_Mesh(cubeAsset, mat1));
        // audio2Actor.AddComponent(new C_AudioSource() { Audio = audioAsset });
        // audio2Actor.Position = new Vector3(-5, 10, -5);

        scene.AddProcessor(new EP_ColorChanger());
        scene.AddProcessor(new EP_Rotator());
        scene.AddService<VoxelWorld>();

        // for (int i = -50; i < 50; i++)
        // {
        //     Actor a = scene.CreateActor();
        //     a.AddComponent(new C_Mesh(triangleAsset, new PBRMaterial(new PBRMaterialProperties() { Color = Color.Orange }) { Pipeline = pipeline }));
        //     a.AddComponent<C_ColorChanger>();
        //     a.Position = new Vector3(i * 0.75f, i * 0.1f, i * 0.5f);
        // }
    }

}
