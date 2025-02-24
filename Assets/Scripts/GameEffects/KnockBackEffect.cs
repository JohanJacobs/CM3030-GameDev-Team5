using UnityEngine;

[CreateAssetMenu(menuName = "Effects/Knock Back")]
public class KnockBackEffect : Effect
{
    public Magnitude KnockBackMagnitude;

    public override void ApplyEffect(EffectInstance effectInstance)
    {
        // direction will be zero if it targets self
        if (effectInstance.SelfTargeted)
            return;

        var asc = effectInstance.Context.Target;
        if (asc == null)
            return;

        var monster = asc.Owner as Monster;
        if (monster == null)
            return;

        var effect = effectInstance.GetEffect<KnockBackEffect>();

        monster.KnockBack(effectInstance.Context.Source.transform.position, effectInstance.CalculateMagnitude(effect.KnockBackMagnitude));
    }
}