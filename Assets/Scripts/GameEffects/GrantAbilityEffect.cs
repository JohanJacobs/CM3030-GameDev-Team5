/*

University of London
BsC Computer Science Course
Games Development
Final Assignment - Streets of Fire Game

Group 5 

Please refer to the README file for detailled information

GrantAbilityEffect.cs

*/

using UnityEngine;

/// <summary>
/// Effect that instantly grants ability to its target when applied
/// </summary>
[CreateAssetMenu(menuName = "Effects/Grant Ability")]
public class GrantAbilityEffect : Effect
{
    public Ability GrantedAbility;

    public override void ApplyEffect(EffectInstance effectInstance)
    {
        base.ApplyEffect(effectInstance);

        effectInstance.Context.Target.AddAbility(GrantedAbility);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // forced to be instant
        DurationPolicy = EffectDurationPolicy.Instant;
    }
#endif
}
