using System.Numerics;
using VoxelEngine.Core;

namespace CustomGame;

internal class B_QuadSpawner : Behavior, IUpdatable
{
    private int amount;
    private float maxspawnTime;
    private float minspawnTime;

    private float timer;
    private float spawnTime;

    public B_QuadSpawner(int amount = 1, float maxspawnTime = 5f, float minspawnTime = 1f)
    {
        this.amount = amount;
        this.maxspawnTime = maxspawnTime;
        this.minspawnTime = minspawnTime;
        SetSpawnTime();
    }

    private void SetSpawnTime()
    {
        spawnTime = EMath.RandomRange(minspawnTime, maxspawnTime);
    }

    public void OnUpdate()
    {
        timer += Time.DeltaTime;
        if (timer > spawnTime)
        {
            Spawn();
            timer = 0;
            SetSpawnTime();
        }
    }

    private void Spawn()
    {
        for (int i = 0; i < amount; i++)
        {
            SpawnQuad();
        }
    }

    private void SpawnQuad()
    {
        Actor a = scene.CreateActor();

        // MaterialAsset mat = new MaterialAsset();
        // mat.Color = Color.Random;

        // a.AddComponent(new C_Mesh(BuildInAssets.QuadID, mat));
        // // a.AddBehavior(new B_ImpactEffect(1f));
        // // a.AddBehavior(new B_Mover()
        // {
        //     dir = EMath.RandomSphere(),
        //     speed = EMath.RandomRange(1f, 2f)
        // });

        Vector2 circle = EMath.RandomCircle();
        a.Position = new Vector3(circle.X * EMath.RandomRange(0.5f, 2f), circle.Y * EMath.RandomRange(0.5f, 2f), 5);
        a.LocalScale = Vector3.One * EMath.RandomRange(0.5f, 1.1f);
    }


}