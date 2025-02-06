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

        effect.Modifiers = new [] { attributeModifier };
        effect.DurationPolicy = EffectDurationPolicy.Instant;

        Effect = effect;
    }
}
