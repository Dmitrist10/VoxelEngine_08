using System.Diagnostics;
using System.Numerics;

namespace VoxelEngine.Core;

/// <summary>
/// System that propagates transforms from parents to children
/// Updates world transforms based on hierarchy and local transforms
/// </summary>
public class EP_Transform : EntityProcessor, IUpdatable
{
    private QueryDescription _queryAll;
    private QueryDescription _queryMatrix;

    public override void OnInitialize()
    {
        _queryAll = new QueryDescription()
            .WithAll<C_Hierarchy, C_Transform>();

        _queryMatrix = new QueryDescription()
            .WithAll<C_Transform, C_WorldTransformMatrix>();
    }


    /// <summary>
    /// Update all transforms if the local pos has changed (IsDirty)
    /// Updates the Hierarchy (when parent moved move the children)
    /// Updates the matrix for dirty transforms
    /// </summary>
    public void OnUpdate()
    {
        // First pass: Update all root actors (world = local)
        world.ParallelQuery(in _queryAll, (ref C_Hierarchy hierarchy, ref C_Transform transform) =>
        {
            if (!hierarchy.IsRoot) return;

            if (transform.IsDirty)
            {
                transform.WorldPosition = transform.LocalPosition;
                transform.WorldRotation = transform.LocalRotation;
                transform.WorldScale = transform.LocalScale;
            }
        });

        // Second pass: Propagate to children depth-first
        world.ParallelQuery(in _queryAll, (Entity entity, ref C_Hierarchy hierarchy) =>
        {
            if (hierarchy.IsRoot && hierarchy.HasChildren)
            {
                PropagateToChildren(entity, ref hierarchy);
            }
        });

        // Third pass: Update matrices for dirty transforms
        UpdateTransformMatrices();
    }

    private void PropagateToChildren(Entity parentEntity, ref C_Hierarchy parentHierarchy)
    {
        ref var parentTransform = ref world.Get<C_Transform>(parentEntity);

        Entity currentChild = parentHierarchy.FirstChild;

        // Iterate through siblings
        while (currentChild != Entity.Null)
        {
            ref var childHierarchy = ref world.Get<C_Hierarchy>(currentChild);
            ref var childTransform = ref world.Get<C_Transform>(currentChild);

            // Only update if parent or child is dirty
            if (parentTransform.IsDirty || childTransform.IsDirty)
            {
                // Calculate world transform from parent + local
                childTransform.WorldRotation = parentTransform.WorldRotation * childTransform.LocalRotation;
                childTransform.WorldScale = parentTransform.WorldScale * childTransform.LocalScale;

                // Position = ParentPos + ParentRot * (ParentScale * LocalPos)
                Vector3 scaledPos = childTransform.LocalPosition * parentTransform.WorldScale;
                Vector3 rotatedPos = Vector3.Transform(scaledPos, parentTransform.WorldRotation);
                childTransform.WorldPosition = parentTransform.WorldPosition + rotatedPos;

                // Keep dirty flag for matrix update
                childTransform.IsDirty = true;
            }

            // Recurse to grandchildren
            if (childHierarchy.HasChildren)
            {
                PropagateToChildren(currentChild, ref childHierarchy);
            }

            // Move to next sibling
            currentChild = childHierarchy.NextSibling;
        }
    }

    private void UpdateTransformMatrices()
    {
        world.ParallelQuery(in _queryMatrix, (ref C_Transform transform, ref C_WorldTransformMatrix matrix) =>
        {
            if (transform.IsDirty)
            {
                // Build TRS matrix (Scale * Rotation * Translation)
                matrix.WorldMatrix = Matrix4x4.CreateScale(transform.WorldScale) *
                                     Matrix4x4.CreateFromQuaternion(transform.WorldRotation) *
                                     Matrix4x4.CreateTranslation(transform.WorldPosition);

                transform.IsDirty = false;
            }
        });
    }
}
