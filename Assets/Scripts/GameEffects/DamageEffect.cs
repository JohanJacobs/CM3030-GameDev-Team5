using System;
using UnityEngine;

public enum DamageCalculation
{
    Magnitude,
    SetByCaller,
}

[CreateAssetMenu(menuName = "Effects/Damage")]
public class DamageEffect : Effect
{
    public DamageCalculation DamageCalculation = DamageCalculation.Magnitude;
    public Magnitude DamageMagnitude;
    public float DamageSetByCaller;

    public override void ApplyEffect(EffectInstance effectInstance)
    {
        var effectContext = effectInstance.Context;

        if (effectContext.Target.Owner.IsDead)
            return;

        float amount;

        switch (DamageCalculation)
        {
            case DamageCalculation.Magnitude:
                amount = DamageMagnitude.Calculate(effectContext);
                break;
            case DamageCalculation.SetByCaller:
                amount = DamageSetByCaller;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        // TODO: check if amount is positive?
        // TODO: damage effect origin could be e.g. projectile, not source owner

        effectContext.Target.AddDamage(amount);

        effectContext.Source.Owner.NotifyDamageDealt(effectContext.Target.Owner);
        effectContext.Target.Owner.NotifyDamageTaken(effectContext.Source.Owner, effectContext.Source.transform.position, amount);
    }
}