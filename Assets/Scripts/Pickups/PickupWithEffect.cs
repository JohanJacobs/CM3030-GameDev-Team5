using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PickupWithEffect : Pickup
{
    public Effect Effect;

    public IReadOnlyDictionary<AttributeType, float> AbsoluteAttributeValueChanges => _absoluteAttributeValueChanges;

    private readonly Dictionary<AttributeType, float> _absoluteAttributeValueChanges = new Dictionary<AttributeType, float>();

    protected override bool HandlePickUpImpl(GameObject target)
    {
        var asc = target.GetComponent<AbilitySystemComponent>();
        if (asc == null)
            return false;

        var attributeInfosBefore = asc.GetModifiedAttributesInfo(Effect.Modifiers)
            .OrderBy(info => info.Attribute)
            .ToArray();

        asc.ApplyEffectToSelf(Effect);

        var attributeInfosAfter= asc.GetModifiedAttributesInfo(Effect.Modifiers)
            .OrderBy(info => info.Attribute)
            .ToArray();

        Debug.Assert(attributeInfosBefore.Length == attributeInfosAfter.Length);

        for (var i = 0; i < attributeInfosAfter.Length; ++i)
        {
            var attributeInfoBefore = attributeInfosBefore[i];
            var attributeInfoAfter = attributeInfosAfter[i];

            Debug.Assert(attributeInfoBefore.Attribute == attributeInfoAfter.Attribute);

            _absoluteAttributeValueChanges.Add(attributeInfoAfter.Attribute, attributeInfoAfter.Value - attributeInfoBefore.Value);
        }

        return true;
    }
}
