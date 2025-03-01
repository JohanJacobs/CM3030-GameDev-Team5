using UnityEngine;

public class GrenadeProjectile : Projectile
{
    public Effect[] HitEffects;

    public Magnitude ExplosionRange;

    public bool ExplodeOnHit = false;

    private bool _exploded = false;
    private bool _landed = false;

    public GrenadeProjectile()
    {
        DestroyOnHit = false;
    }

    protected override void OnCollisionEnterImpl(Collision collision)
    {
        base.OnCollisionEnterImpl(collision);

        // mark as landed on first collision with anything
        _landed = true;
    }

    protected override bool HandleHit(Collider other)
    {
        Land(other);

        if (ExplodeOnHit)
        {
            Explode();
            return true;
        }

        return false;
    }

    protected override void OnEndOfLife()
    {
        // triggers explosions as if triggered by fuse
        Explode();
    }

    private void Land(Collider other)
    {
        if (_landed)
            return;

        _landed = true;

        var creature = other.GetComponentInParent<Creature>();
        if (creature)
        {
            // TODO: use AbilityTargetUtility
            foreach (var effect in HitEffects)
            {
                AbilityInstance.AbilitySystemComponent.ApplyEffectToTarget(effect, creature.AbilitySystemComponent, AbilityInstance);
            }
        }
    }

    private void Explode()
    {
        if (_exploded)
            return;

        _exploded = true;

        var range = AbilityInstance.CalculateMagnitude(ExplosionRange);

        var targetQuery = new AbilityTargetQuery()
        {
            LayerMask = LayerMask,
            Origin = transform.position,
            Range = range,
        };

        var targets = AbilityTargetSelector.GetAreaTargets(targetQuery);

        if (AbilityInstance.Ability is AttackAbility attackAbility)
        {
            attackAbility.ApplyProjectileDamageEffect(AbilityInstance, this, targets);
        }

        SpawnExplosionVisuals();
    }

    private void SpawnExplosionVisuals()
    {

    }
}