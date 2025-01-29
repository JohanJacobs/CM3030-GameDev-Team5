using UnityEngine;
using System.Collections.Generic;

public class ExperienceOrbPickup : PickupWithEffect
{
    public float Experience;

    private void Start()
    {
        var attributeModifier = ScriptableObject.CreateInstance<AttributeModifier>();

        attributeModifier.Attribute = AttributeType.Experience;
        attributeModifier.Type = AttributeModifierType.Add;
        attributeModifier.Value = Experience;
        attributeModifier.Post = false;
        attributeModifier.Permanent = true;

        var effect = ScriptableObject.CreateInstance<Effect>();

        effect.Modifiers = new List<AttributeModifier> { attributeModifier };
        effect.DurationPolicy = EffectDurationPolicy.Instant;
        effect.ApplicationPolicy = EffectApplicationPolicy.Instant;
        effect.CancellationPolicy = EffectCancellationPolicy.DoNothing;

        Effect = effect;
    }
}
