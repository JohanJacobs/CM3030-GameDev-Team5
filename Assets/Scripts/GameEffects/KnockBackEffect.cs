using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Effects/Knock Back")]
public class KnockBackEffect : Effect
{
    public static readonly Tag AmountSetByCaller = new Tag("KnockBackEffect.Amount");
    public static readonly Tag OriginSetByCaller = new Tag("KnockBackEffect.Origin");

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

        var monster = effectContext.Target.Owner as Monster;
        if (monster == null)
            return;

        // TODO: check if amount is positive?

        monster.KnockBack(origin, amount);
    }
}