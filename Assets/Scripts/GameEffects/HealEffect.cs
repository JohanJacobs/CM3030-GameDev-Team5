using System;
using UnityEngine;

public enum HealCalculation
{
    Magnitude,
    SetByCaller,
}

[CreateAssetMenu(menuName = "Effects/Heal")]
public class HealEffect : Effect
{
    public HealCalculation HealCalculation = HealCalculation.Magnitude;
    public Magnitude HealMagnitude;
    public float HealSetByCaller;

    public override void ApplyEffect(EffectInstance effectInstance)
    {
        var effectContext = effectInstance.Context;

        if (effectContext.Target.Owner.IsDead)
            return;

        float amount;

        switch (HealCalculation)
        {
            case HealCalculation.Magnitude:
                amount = HealMagnitude.Calculate(effectContext);
                break;
            case HealCalculation.SetByCaller:
                amount = HealSetByCaller;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        // TODO: check if amount is positive?

        effectContext.Target.AddHealing(amount);
    }
}