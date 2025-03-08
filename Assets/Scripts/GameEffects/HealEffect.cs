/*

University of London
BsC Computer Science Course
Games Design
Final Assignment - Streets of Fire Game

Group 5 

Please view README file for detailed information

*/
using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Effects/Heal")]
public class HealEffect : Effect
{
    public static readonly Tag AmountSetByCaller = new Tag("HealEffect.Amount");

    public EffectAmountCalculation AmountCalculation = EffectAmountCalculation.Magnitude;
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

        effectContext.Target.AddHealing(amount);
    }
}