using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Effects/Damage")]
public class DamageEffect : Effect
{
    public static readonly Tag AmountSetByCaller = new Tag("DamageEffect.Amount");
    public static readonly Tag OriginSetByCaller = new Tag("DamageEffect.Origin");

    public EffectAmountCalculation AmountCalculation = EffectAmountCalculation.Magnitude;
    public EffectOriginCalculation OriginCalculation = EffectOriginCalculation.Source;
    public Magnitude Amount;

    public override void ApplyEffect(EffectInstance effectInstance)
    {
        var effectContext = effectInstance.Context;

        if (effectContext.Target.Owner.IsDead)
            return;

        float amount;

        switch (AmountCalculation)
        {
            case EffectAmountCalculation.Magnitude:
                amount = effectInstance.CalculateMagnitude(Amount);
                break;
            case EffectAmountCalculation.SetByCaller:
                effectContext.GetValue(AmountSetByCaller, out amount);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        // TODO: check if amount is positive?

        effectContext.Target.AddDamage(amount);

        Vector3 origin;

        switch (OriginCalculation)
        {
            case EffectOriginCalculation.Source:
                origin = effectContext.Source.transform.position;
                break;
            case EffectOriginCalculation.Target:
                origin = effectContext.Target.transform.position;
                break;
            case EffectOriginCalculation.SetByCaller:
                effectContext.GetValue(OriginSetByCaller, out origin);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        effectContext.Source.Owner.NotifyDamageDealt(effectContext.Target.Owner, origin, amount);
        effectContext.Target.Owner.NotifyDamageTaken(effectContext.Source.Owner, origin, amount);
    }
}