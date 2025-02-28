using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Effects/Damage")]
public class DamageEffect : Effect
{
    public static readonly Tag AmountSetByCaller = "DamageEffect.Amount";
    public static readonly Tag OriginSetByCaller = "DamageEffect.Origin";
    public static readonly Tag CauserSetByCaller = "DamageEffect.Causer";
    public static readonly Tag SelfDestruct = "DamageEffect.SelfDestruct";

    public EffectAmountCalculation AmountCalculation = EffectAmountCalculation.Magnitude;
    public EffectOriginCalculation OriginCalculation = EffectOriginCalculation.Source;
    public Magnitude Amount;

    public override void ApplyEffect(EffectInstance effectInstance)
    {
        var effectContext = effectInstance.Context;

        if (effectContext.Target.Owner.IsDead)
            return;

        float amount;
        Vector3 origin;
        GameObject causer;

        if (effectInstance.Context.HasValue(SelfDestruct))
        {
            amount = effectContext.Target.Owner.Health;
            origin = effectContext.Target.Owner.transform.position;
            causer = effectContext.Target.Owner.gameObject;
        }
        else
        {
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

            if (effectContext.GetValue(CauserSetByCaller, out causer))
            {
                Debug.Assert(causer);
            }
            else
            {
                causer = effectContext.Source.Owner.gameObject;
            }
        }

        var damageEvent = new DamageEvent()
        {
            AbilityInstance = effectContext.AbilityInstance,
            Instigator = effectContext.Source.Owner.gameObject,
            Causer = causer,
            Victim = effectContext.Target.Owner.gameObject,
            Damage = amount,
            Critical = false,
            Origin = origin,
        };

        // TODO: check if amount is positive?

        effectContext.Target.AddDamage(amount);

        effectContext.Source.Owner.NotifyDamageDealt(effectContext.Target.Owner, origin, amount);
        effectContext.Target.Owner.NotifyDamageTaken(effectContext.Source.Owner, origin, amount);
    }
}