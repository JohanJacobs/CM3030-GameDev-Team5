using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct AbilityTargetQuery
{
    public LayerMask LayerMask;
    public Vector3 Origin;
    public Vector3 Direction;
    public float Range;
}

public struct AbilityTarget
{
    public Collider Collider;
    public AbilitySystemComponent AbilitySystemComponent;
    public RaycastHit RaycastHit;
    public float Distance;
}

public static class AbilityTargetSelector
{
    private const int MaxTargets = 128;

    private class TargetSelectorData
    {
        public readonly Collider[] Colliders = new Collider[MaxTargets];
        public readonly RaycastHit[] RaycastHits = new RaycastHit[MaxTargets];
        public readonly AbilityTarget[] Targets = new AbilityTarget[MaxTargets];

        public void Clear()
        {
            Array.Clear(Colliders, 0, Colliders.Length);
            Array.Clear(RaycastHits, 0, RaycastHits.Length);
            Array.Clear(Targets, 0, Targets.Length);
        }
    }

    private class RaycastHitDistanceComparer : IComparer<RaycastHit>
    {
        public int Compare(RaycastHit lhs, RaycastHit rhs)
        {
            var diff = lhs.distance - rhs.distance;
            if (diff < 0)
                return -1;
            if (diff > 0)
                return 1;
            return 0;
        }
    }

    private static readonly TargetSelectorData DefaultTargetSelectorData = new TargetSelectorData();
    private static readonly RaycastHitDistanceComparer TargetRaycastHitDistanceComparer = new RaycastHitDistanceComparer();

    public static IEnumerable<AbilityTarget> GetSectorTargets(in AbilityTargetQuery query, float degrees, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
    {
        var data = AllocateTargetSelectorData();

        var colliderCount = Physics.OverlapSphereNonAlloc(query.Origin, query.Range, data.Colliders, query.LayerMask, queryTriggerInteraction);

        var queryDirection = query.Direction;

        queryDirection.y = 0;
        queryDirection.Normalize();

        var dotThreshold = Mathf.Cos(degrees * Mathf.Deg2Rad);

        int targetIndex = 0;

        foreach (var collider in data.Colliders.Take(colliderCount))
        {
            var delta = collider.transform.position - query.Origin;
            var direction = delta;

            direction.y = 0;
            direction.Normalize();

            if (Vector3.Dot(queryDirection, direction) < dotThreshold)
                continue;

            ref var target = ref data.Targets[targetIndex];

            target.Collider = collider;
            target.AbilitySystemComponent = collider.GetComponentInParent<AbilitySystemComponent>();
            target.Distance = delta.magnitude;

            ++targetIndex;
        }

        return data.Targets.Take(targetIndex);
    }

    public static IEnumerable<AbilityTarget> GetRaycastTargets(in AbilityTargetQuery query, int maxTargets, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
    {
        var data = AllocateTargetSelectorData();

        var hitCount = Physics.RaycastNonAlloc(query.Origin, query.Direction, data.RaycastHits, query.Range, query.LayerMask, queryTriggerInteraction);
        if (hitCount > 1)
        {
            Array.Sort(data.RaycastHits, 0, hitCount, TargetRaycastHitDistanceComparer);
        }

        int targetIndex = 0;

        foreach (var raycastHit in data.RaycastHits.Take(Mathf.Clamp(maxTargets, 1, hitCount)))
        {
            ref var target = ref data.Targets[targetIndex];

            target.Collider = raycastHit.collider;
            target.AbilitySystemComponent = raycastHit.collider.GetComponentInParent<AbilitySystemComponent>();
            target.RaycastHit = raycastHit;
            target.Distance = raycastHit.distance;

            ++targetIndex;
        }

        return data.Targets.Take(targetIndex);
    }

    public static IEnumerable<AbilityTarget> GetRaycastTargetsSingle(in AbilityTargetQuery query, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
    {
        return GetRaycastTargets(query, 1, queryTriggerInteraction);
    }

    public static IEnumerable<AbilityTarget> GetAreaTargets(in AbilityTargetQuery query, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
    {
        var data = AllocateTargetSelectorData();

        var colliderCount = Physics.OverlapSphereNonAlloc(query.Origin, query.Range, data.Colliders, query.LayerMask, queryTriggerInteraction);

        int targetIndex = 0;

        foreach (var collider in data.Colliders.Take(colliderCount))
        {
            ref var target = ref data.Targets[targetIndex];

            target.Collider = collider;
            target.AbilitySystemComponent = collider.GetComponentInParent<AbilitySystemComponent>();
            target.Distance = Vector3.Distance(query.Origin, collider.transform.position);

            ++targetIndex;
        }

        return data.Targets.Take(targetIndex);
    }

    private static TargetSelectorData AllocateTargetSelectorData()
    {
        // might need several instances of this in the future, can be cycled with ring buffer
        DefaultTargetSelectorData.Clear();

        return DefaultTargetSelectorData;
    }
}