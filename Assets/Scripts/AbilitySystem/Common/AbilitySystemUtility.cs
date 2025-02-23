using UnityEngine;

public static class AbilitySystemUtility
{
    private static AbilitySystemComponent GetAbilitySystemComponent(GameObject gameObject)
    {
        return gameObject.GetComponent<AbilitySystemComponent>();
    }

    public static float GetAttributeValueOrDefault(GameObject gameObject, AttributeType attribute, float defaultValue = 0)
    {
        var asc = GetAbilitySystemComponent(gameObject);
        if (!asc)
            return defaultValue;

        var value = asc.GetAttributeValue(attribute, out var attributeExists);
        if (!attributeExists)
            return defaultValue;

        return value;
    }
}