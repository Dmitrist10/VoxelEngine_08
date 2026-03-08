using System.Numerics;
using BepuPhysics;
using BepuPhysics.Collidables;
using BepuPhysics.CollisionDetection;
using BepuUtilities;
using BepuUtilities.Memory;
using VoxelEngine.Core;
using VoxelEngine.Graphics;

namespace VoxelEngine.Physics;

public class Physics3DModule : IEngineModule
{
    public Simulation Simulation { get; private set; } = null!;
    public BufferPool BufferPool { get; private set; } = null!;

    public void OnInitialize()
    {
        BufferPool = new BufferPool();

        // Simple initialization, a real app might need more complex callbacks
        Simulation = Simulation.Create(
            BufferPool,
            new NarrowPhaseCallbacks(),
            new PoseIntegratorCallbacks(new Vector3(0, -9.81f, 0)),
            new SolveDescription(8, 1));

        ServiceContainer.Register(this);
    }

    public void OnLoad(IWindowSurface window)
    {
    }

    public void OnUpdate()
    {
        // Get delta time from engine context if possible, otherwise use fixed step
        // For simplicity, we assume a 60hz step here or read from Time class
        Simulation.Timestep(1f / 60f);
    }

    public void Cleanup()
    {
        Simulation.Dispose();
        BufferPool.Clear();
    }
}

// Basic default callbacks for BepuPhysics 2.x
struct NarrowPhaseCallbacks : INarrowPhaseCallbacks
{
    public void Initialize(Simulation simulation) { }

    public bool AllowContactGeneration(int workerIndex, CollidableReference a, CollidableReference b, ref float speculativeMargin)
    {
        return a.Mobility == CollidableMobility.Dynamic || b.Mobility == CollidableMobility.Dynamic;
    }

    public bool AllowContactGeneration(int workerIndex, CollidablePair pair, int childIndexA, int childIndexB)
    {
        return true;
    }

    public bool ConfigureContactManifold<TManifold>(int workerIndex, CollidablePair pair, ref TManifold manifold, out PairMaterialProperties pairMaterial) where TManifold : unmanaged, IContactManifold<TManifold>
    {
        pairMaterial.FrictionCoefficient = 1f;
        pairMaterial.MaximumRecoveryVelocity = 2f;
        pairMaterial.SpringSettings = new BepuPhysics.Constraints.SpringSettings(30, 1);
        return true;
    }

    public bool ConfigureContactManifold(int workerIndex, CollidablePair pair, int childIndexA, int childIndexB, ref ConvexContactManifold manifold)
    {
        return true;
    }

    public void Dispose() { }
}

struct PoseIntegratorCallbacks : IPoseIntegratorCallbacks
{
    public Vector3 Gravity;
    public float LinearDamping;
    public float AngularDamping;
    public Vector3 GravityDt;

    public readonly AngularIntegrationMode AngularIntegrationMode => AngularIntegrationMode.Nonconserving;
    public readonly bool AllowSubstepsForUnconstrainedBodies => false;
    public readonly bool IntegrateVelocityForKinematics => false;

    public PoseIntegratorCallbacks(Vector3 gravity, float linearDamping = .03f, float angularDamping = .03f)
    {
        Gravity = gravity;
        LinearDamping = linearDamping;
        AngularDamping = angularDamping;
        GravityDt = default;
    }

    public void Initialize(Simulation simulation) { }

    public void PrepareForIntegration(float dt)
    {
        GravityDt = Gravity * dt;
    }

    public void IntegrateVelocity(Vector<int> bodyIndices, Vector3Wide position, QuaternionWide orientation, BodyInertiaWide localInertia, Vector<int> integrationMask, int workerIndex, Vector<float> dt, ref BodyVelocityWide velocity)
    {
        // dt is a Vector<float>, so we can just extract the first lane for standard Broadcast scalar usage or just use dt directly if Vector3Wide has a constructor for it.
        // Actually since we just need the float timestep dt[0]:
        float dtScalar = dt[0];
        Vector3Wide gravityDt = Vector3Wide.Broadcast(Gravity * dtScalar);
        Vector3Wide linearDampingFallback = Vector3Wide.Broadcast(new Vector3(MathF.Max(0, 1 - LinearDamping)));
        Vector3Wide angularDampingFallback = Vector3Wide.Broadcast(new Vector3(MathF.Max(0, 1 - AngularDamping)));

        velocity.Linear = velocity.Linear * linearDampingFallback + gravityDt;
        velocity.Angular = velocity.Angular * angularDampingFallback;
    }
}
