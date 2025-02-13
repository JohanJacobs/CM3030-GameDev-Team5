using System;
using UnityEngine;

public class AbilityInstance
{
    public AbilitySystemComponent AbilitySystemComponent => _weakAbilitySystemComponent.TryGetTarget(out var asc) ? asc : null;
    public Ability Ability { get; }
    public bool Active => _active;

    public float CooldownTimeRemainingFraction
    {
        get
        {
            if (Ability.CooldownEffect == null)
                return 1f;

            // TODO: pick largest if there are multiple?
            return AbilitySystemComponent.GetActiveEffectTimeRemainingFraction(Ability.CooldownEffect);
        }
    }

    private readonly WeakReference<AbilitySystemComponent> _weakAbilitySystemComponent;

    private AbilityLogic _abilityLogic;

    private EffectHandle _costEffectHandle;
    private EffectHandle _cooldownEffectHandle;

    private bool _active;

    public AbilityInstance(AbilitySystemComponent asc, Ability ability)
    {
        Ability = ability;

        _weakAbilitySystemComponent = new WeakReference<AbilitySystemComponent>(asc);

        _abilityLogic = ability.AbilityLogicClass.CreateInstance();
    }

    public void Destroy()
    {
        // NOTE: required to break cross-reference chain
        _abilityLogic = null;
    }

    public bool TryActivate()
    {
        if (_active)
            return true;

        if (!Commit())
            return false;

        Activate();

        return true;
    }

    public bool CanActivateAbility()
    {
        if (_active)
            return false;

        if (!CheckCooldown())
            return false;
        if (!CheckCost())
            return false;

        if (!_abilityLogic.CanActivateAbility(this))
            return false;

        return true;
    }

    private void Activate()
    {
        if (_active)
            return;

        _active = true;

        _abilityLogic.ActivateAbility(this);
    }

    public void End()
    {
        if (!_active)
            return;

        _active = false;

        _abilityLogic.EndAbility(this);
    }

    private bool Commit()
    {
        if (!CheckCooldown())
            return false;
        if (!CheckCost())
            return false;

        if (!_abilityLogic.CommitAbility(this))
            return false;

        var asc = AbilitySystemComponent;

        if (Ability.CostEffect != null)
        {
            _costEffectHandle = asc.ApplyEffectToSelf(Ability.CostEffect);
        }

        if (Ability.CooldownEffect != null)
        {
            _cooldownEffectHandle = asc.ApplyEffectToSelf(Ability.CooldownEffect);
        }

        return true;
    }

    public void Update(float deltaTime)
    {
        if (!_active)
            return;

        _abilityLogic.UpdateAbility(this, deltaTime);
    }

    private bool CheckCost()
    {
        if (Ability.CostEffect == null)
            return true;

        var asc = AbilitySystemComponent;

        foreach (var modifier in Ability.CostEffect.Modifiers)
        {
            modifier.Attribute.EnsureCanHaveModifiers();

            if (modifier.Type == NewAttributeModifierType.Override)
                throw new InvalidOperationException($"Cost effect must not have Override modifiers ({modifier.Attribute.GetName()})");

            var value = asc.GetAttributeValue(modifier.Attribute);
            // no resource to spend
            if (value < 0f)
                return false;

            var valueWithCostModifier = asc.GetAttributeValueWithExtraModifier(modifier.Attribute, ScalarModifier.MakeFromAttributeModifier(modifier), modifier.Post);
            // too expensive
            if (valueWithCostModifier < 0f)
                return false;

            var absoluteCost = valueWithCostModifier - value;
            if (absoluteCost > 0f)
                throw new InvalidOperationException($"Cost effect modifier must not be positive ({modifier.Attribute.GetName()})");
        }

        return true;
    }

    private bool CheckCooldown()
    {
        if (Ability.CooldownEffect == null)
            return true;

        if (_cooldownEffectHandle.Active)
            return false;

        if (AbilitySystemComponent.HasActiveEffect(Ability.CooldownEffect))
            return false;

        return true;
    }
}
