/*

University of London
BsC Computer Science Course
Games Development
Final Assignment - Streets of Fire Game

Group 5 

Please refer to the README file for detailled information

AbilityInstance.cs

*/
using System;
using UnityEngine;

public class AbilityInstance
{
    public AbilitySystemComponent AbilitySystemComponent { get; private set; }
    public Creature Owner { get; private set; }
    public Ability Ability { get; private set; }
    public Tag InputTag { get; set; }
    public bool Active => _active;
    public AbilityInstanceData Data => _data;

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

    private IAbilityLogic _abilityLogic;

    private EffectHandle _costEffectHandle;
    private EffectHandle _cooldownEffectHandle;

    private AbilityInstanceData _data;

    private bool _active;
    private bool _inputPressed;

    public AbilityInstance(AbilitySystemComponent asc, Ability ability)
    {
        AbilitySystemComponent = asc;
        Owner = asc.GetComponent<Creature>();
        Ability = ability;
        InputTag = ability.InputTag;

        _abilityLogic = ability;

        _data = ability.AbilityInstanceDataClass.CreateInstance();
    }

    public void NotifyAdded()
    {
        _abilityLogic.HandleAbilityAdded(this);
    }

    public void NotifyRemoved()
    {
        _abilityLogic.HandleAbilityRemoved(this);
    }

    public void Destroy()
    {
        Debug.Assert(!Active, "Destroying active ability instance probably points to logic error");

        AbilitySystemComponent = null;
        Owner = null;
        Ability = null;

        // NOTE: might be required to break cross-reference chain
        _abilityLogic = null;

        _data = null;
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

    public T GetData<T>() where T : AbilityInstanceData
    {
        if (_data is T concreteData)
            return concreteData;

        throw new InvalidOperationException($"Requested invalid ability instance data type {typeof(T)}");
    }

    public T GetAbility<T>() where T : Ability
    {
        if (Ability is T concreteAbility)
            return concreteAbility;

        throw new InvalidOperationException($"Requested invalid ability type {typeof(T)}");
    }

    public float CalculateMagnitude(Magnitude magnitude)
    {
        return magnitude.Calculate(this);
    }

    private void Activate()
    {
        if (_active)
            return;

        _active = true;

        _abilityLogic.ActivateAbility(this);

        if (Ability.InputMappingContext)
        {
            var inputComponent = AbilitySystemComponent.GetComponent<InputComponent>();
            if (inputComponent)
            {
                inputComponent.AddInputMappingContext(Ability.InputMappingContext);
            }
        }
    }

    public void End()
    {
        if (!_active)
            return;

        _active = false;

        _abilityLogic.EndAbility(this);

        if (Ability.InputMappingContext)
        {
            var inputComponent = AbilitySystemComponent.GetComponent<InputComponent>();
            if (inputComponent)
            {
                inputComponent.RemoveInputMappingContext(Ability.InputMappingContext);
            }
        }
    }

    public void Abort()
    {
        // TODO: implement abortion logic

        End();
    }

    public void NotifyInputActionPressed(InputAction action)
    {
        if (action.Tag == InputTag)
        {
            bool tryActivate = Ability.InputPolicy == AbilityInputPolicy.TryActivateOnInputPressed && !_inputPressed;

            _inputPressed = true;

            if (tryActivate)
            {
                TryActivate();
            }
        }

        if (_active)
        {
            _abilityLogic.HandleAbilityInputActionPressed(this, action);
        }
    }

    public void NotifyInputActionReleased(InputAction action)
    {
        if (action.Tag == InputTag)
        {
            _inputPressed = false;
        }

        if (_active)
        {
            _abilityLogic.HandleAbilityInputActionReleased(this, action);
        }
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
        {
            bool tryActivate = Ability.InputPolicy == AbilityInputPolicy.TryActivateWhileInputPressed && _inputPressed && !_active;

            if (tryActivate)
            {
                TryActivate();
            }
        }

        if (_active)
        {
            _abilityLogic.UpdateAbility(this, deltaTime);
        }
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

        if (_cooldownEffectHandle?.Active ?? false)
            return false;

        if (AbilitySystemComponent.HasActiveEffect(Ability.CooldownEffect))
            return false;

        return true;
    }
}
