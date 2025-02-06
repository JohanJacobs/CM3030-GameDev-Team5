using UnityEngine;
using System;

[Serializable]
public sealed class AbilityCostDefinition
{
    public AttributeType Attribute;
    public float Value;
}

[Serializable]
public sealed class AbilityCooldownDefinition
{
    public float Value;
}

[CreateAssetMenu]
public class Ability : ScriptableObject
{
    public bool ActivateOnGranted = false;

    public Effect CostEffect;
    public Effect CooldownEffect;

    public Tag[] Tags;
    public Tag[] BlockTags;
    public Tag[] CancelTags;

    public virtual bool TryActivateAbility()
    {
        if (!CommitAbility())
        {
            EndAbility();
            return false;
        }

        ActivateAbility();

        return true;
    }

    public virtual void ActivateAbility()
    {

    }

    public virtual void EndAbility()
    {

    }

    public virtual bool CommitAbility()
    {
        return true;
    }
}
