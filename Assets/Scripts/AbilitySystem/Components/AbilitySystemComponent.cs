using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AbilitySystemComponent : MonoBehaviour
{
    public delegate void SelfDelegate(AbilitySystemComponent asc);

    private static readonly AttributeType[] AllAttributes = Enum.GetValues(typeof(AttributeType)).Cast<AttributeType>().ToArray();

    public IReadOnlyCollection<Tag> Tags => _tags;

    [SerializeField]
    private AttributeSet[] _grantedAttributeSets;
    [SerializeField]
    private Ability[] _grantedAbilities;

    private readonly List<AttributeSetInstance> _attributeSetInstances = new List<AttributeSetInstance>();
    private readonly EnumArray<AttributeValue, AttributeType> _attributeValues = new EnumArray<AttributeValue, AttributeType>();
    private readonly EnumArray<AttributeModifierStack, AttributeType> _attributeModifiers = new EnumArray<AttributeModifierStack, AttributeType>();

    private readonly List<EffectInstance> _effectInstances = new List<EffectInstance>();

    private readonly List<AbilityInstance> _abilityInstances = new List<AbilityInstance>();

    private readonly TagContainer _tags = new TagContainer();

    private bool _ready;

    private event SelfDelegate Ready;

    public void OnReady(SelfDelegate callback)
    {
        if (_ready)
        {
            callback.Invoke(this);
        }
        else
        {
            Ready += callback;
        }
    }

    #region Attribute Sets

    public void AddAttributeSet(AttributeSet attributeSet)
    {
        var index = _attributeSetInstances.FindIndex(asi => asi.Template == attributeSet);
        if (index >= 0)
            return;

        var attributeSetInstance = attributeSet.CreateInstance();

        _attributeSetInstances.Add(attributeSetInstance);

        foreach (var attributeValue in attributeSetInstance.AttributeValues)
        {
            var existingAttributeValue = _attributeValues[attributeValue.Attribute];
            if (existingAttributeValue != null)
                throw new InvalidOperationException($"Ambiguous attribute {attributeValue.Attribute.GetName()}");

            _attributeValues[attributeValue.Attribute] = attributeValue;

            UpdateAttributeValue(attributeValue);

            attributeValue.BaseValueChanged += OnAttributeBaseValueChanged;
        }
    }

    public void RemoveAttributeSet(AttributeSet attributeSet)
    {
        var index = _attributeSetInstances.FindIndex(asi => asi.Template == attributeSet);
        if (index < 0)
            return;

        var attributeSetInstance = _attributeSetInstances[index];

        foreach (var attributeValue in attributeSetInstance.AttributeValues)
        {
            var existingAttributeValue = _attributeValues[attributeValue.Attribute];
            if (existingAttributeValue != attributeValue)
                throw new InvalidOperationException($"Ambiguous attribute {attributeValue.Attribute.GetName()}");

            _attributeValues[attributeValue.Attribute] = null;

            attributeValue.BaseValueChanged -= OnAttributeBaseValueChanged;
        }

        _attributeSetInstances.RemoveAt(index);
    }

    #endregion

    #region Attribute Values

    public AttributeValue GetAttributeValueObject(AttributeType attribute)
    {
        return _attributeValues[attribute];
    }

    public void SetAttributeBaseValue(AttributeType attribute, float value)
    {
        var attributeValue = _attributeValues[attribute];
        if (attributeValue == null)
            return;

        attributeValue.BaseValue = value;
    }

    public float GetAttributeBaseValue(AttributeType attribute)
    {
        var attributeValue = _attributeValues[attribute];

        return attributeValue?.BaseValue ?? 0f;
    }

    public float GetAttributeValue(AttributeType attribute)
    {
        var attributeValue = _attributeValues[attribute];

        return attributeValue?.Value ?? 0f;
    }

    public float GetAttributeValueWithExtraModifier(AttributeType attribute, ScalarModifier scalarModifier, bool post)
    {
        var attributeValue = _attributeValues[attribute];
        if (attributeValue == null)
            return 0f;

        var attributeModifierStack = _attributeModifiers[attribute];

        return attributeModifierStack.CalculateWithExtraModifier(attributeValue.BaseValue, scalarModifier, post);
    }

    #endregion

    #region Attribute Modifiers

    public AttributeModifierHandle ApplyAttributeModifier(AttributeModifier attributeModifier)
    {
        var attributeModifierStack = _attributeModifiers[attributeModifier.Attribute];

        var attributeModifierInstance = attributeModifierStack.AddModifier(attributeModifier);

        var handle = new AttributeModifierHandle(this, attributeModifierStack, attributeModifierInstance);

        return handle;
    }

    public AttributeModifierHandle ApplyAttributeModifier(AttributeType attribute, ScalarModifier scalarModifier, bool post)
    {
        var attributeModifierStack = _attributeModifiers[attribute];

        var attributeModifierInstance = attributeModifierStack.AddModifier(scalarModifier, post);

        var handle = new AttributeModifierHandle(this, attributeModifierStack, attributeModifierInstance);

        return handle;
    }

    public void CancelAttributeModifier(AttributeModifierHandle handle)
    {
        if (handle.AbilitySystemComponent != this)
            throw new InvalidOperationException("Invalid attribute modifier handle");

        var attributeModifierStack = handle.ModifierStack;
        // NOTE: shouldn't be the case ever but sanity check won't hurt
        if (attributeModifierStack == null)
            return;

        var attributeModifierInstance = handle.ModifierInstance;
        if (attributeModifierInstance == null)
            return;

        attributeModifierStack.RemoveModifier(attributeModifierInstance);

        handle.Clear();
    }

    public void CollapseAttributeModifiers(AttributeType attribute)
    {
        var attributeValue = _attributeValues[attribute];
        if (attributeValue == null)
            return;

        var attributeModifierStack = _attributeModifiers[attribute];

        attributeValue.Reset(attributeModifierStack.Calculate(attributeValue.BaseValue));

        attributeModifierStack.Reset();
    }

    #endregion

    public EffectHandle ApplyEffectToSelf(Effect effect)
    {
        Debug.Assert(effect.IsInstant || effect.IsFinite || effect.IsInfinite);

        var context = new EffectContext(this, this);

        return context.Target.ApplyEffect(effect, context);
    }

    public EffectHandle ApplyEffectToTarget(Effect effect, AbilitySystemComponent target)
    {
        Debug.Assert(effect.IsInstant || effect.IsFinite || effect.IsInfinite);

        var context = new EffectContext(this, target);

        return context.Target.ApplyEffect(effect, context);
    }

    public void CancelEffect(EffectHandle handle)
    {
        if (handle.AbilitySystemComponent != this)
            throw new InvalidOperationException("Invalid effect handle");

        var effectInstance = handle.EffectInstance;
        if (effectInstance == null)
            return;

        if (_effectInstances.Remove(effectInstance))
        {
            effectInstance.Cancel();
        }

        handle.Clear();
    }

    public EffectInstance FindActiveEffect(Effect effect)
    {
        return _effectInstances.Find(effectInstance => effectInstance.Effect == effect);
    }

    public bool HasActiveEffect(Effect effect)
    {
        var effectInstance = FindActiveEffect(effect);

        return !(effectInstance?.Expired ?? true);
    }

    public float GetActiveEffectTimeRemainingFraction(Effect effect)
    {
        var effectInstance = FindActiveEffect(effect);

        return effectInstance?.TimeRemainingFraction ?? 0f;
    }

    public AbilityHandle AddAbility(Ability ability)
    {
        var existingAbilityInstance = _abilityInstances.Find(ai => ai.Ability == ability);
        if (existingAbilityInstance != null)
        {
            return new AbilityHandle(this, existingAbilityInstance);
        }

        var abilityInstance = new AbilityInstance(this, ability);

        _abilityInstances.Add(abilityInstance);

        abilityInstance.NotifyAdded();

        var handle = new AbilityHandle(this, abilityInstance);

        if (ability.ActivateOnGranted)
        {
            ActivateAbility(handle);
        }

        return handle;
    }

    public void RemoveAbility(AbilityHandle handle)
    {
        var abilityInstance = GetAbilityInstanceChecked(handle);
        if (abilityInstance == null)
            return;

        abilityInstance.End();
        abilityInstance.Destroy();

        _abilityInstances.Remove(abilityInstance);

        abilityInstance.NotifyRemoved();
    }

    public void ActivateAbility(AbilityHandle handle)
    {
        var abilityInstance = GetAbilityInstanceChecked(handle);
        if (abilityInstance == null)
            return;

        if (_tags.ContainsAny(abilityInstance.Ability.BlockTags))
            return;

        abilityInstance.TryActivate();
    }

    public void EndAbility(AbilityHandle handle)
    {
        var abilityInstance = GetAbilityInstanceChecked(handle);
        if (abilityInstance == null)
            return;

        abilityInstance.End();
    }

    private void Start()
    {
        foreach (var attribute in AllAttributes)
        {
            var stack = new AttributeModifierStack(attribute);

            stack.Changed += OnAttributeModifierStackChanged;

            _attributeModifiers[attribute] = stack;
        }

        foreach (var attributeSet in _grantedAttributeSets)
        {
            AddAttributeSet(attributeSet);
        }

        foreach (var ability in _grantedAbilities)
        {
            AddAbility(ability);
        }

        _ready = true;

        Ready?.Invoke(this);
    }

    private void Update()
    {
        var deltaTime = Time.deltaTime;

        UpdateActiveEffects(deltaTime);
        UpdateActiveAbilities(deltaTime);
    }

    private void OnDestroy()
    {
        foreach (var abilityInstance in _abilityInstances)
        {
            abilityInstance.Destroy();
        }

        foreach (var attributeModifierInstance in _attributeModifiers)
        {
            attributeModifierInstance.Reset();
        }

        _attributeSetInstances.Clear();

        _attributeValues.Fill(null);

        _effectInstances.Clear();

        _abilityInstances.Clear();

        _tags.Clear();
    }

    private void OnAttributeBaseValueChanged(AttributeValue attributeValue, float oldValue, float newValue)
    {
        UpdateAttributeValue(attributeValue);
    }

    private void OnAttributeModifierStackChanged(AttributeModifierStack attributeModifierStack)
    {
        var attributeValue = _attributeValues[attributeModifierStack.Attribute];
        if (attributeValue == null)
            return;

        UpdateAttributeValue(attributeValue, attributeModifierStack);
    }

    private void UpdateAttributeValue(AttributeValue attributeValue)
    {
        var attributeModifierStack = _attributeModifiers[attributeValue.Attribute];

        UpdateAttributeValue(attributeValue, attributeModifierStack);
    }

    private void UpdateAttributeValue(AttributeValue attributeValue, AttributeModifierStack attributeModifierStack)
    {
        attributeValue.Value = attributeModifierStack.Calculate(attributeValue.BaseValue);
    }

    private EffectHandle ApplyEffect(Effect effect, EffectContext context)
    {
        var effectInstance = new EffectInstance(effect, context);

        effectInstance.Apply();

        if (effectInstance.Expired)
        {
            effectInstance.Cancel();

            return null;
        }

        _effectInstances.Add(effectInstance);

        var handle = new EffectHandle(this, effectInstance);

        return handle;
    }

    private void UpdateActiveEffects(float deltaTime)
    {
        foreach (var effectInstance in _effectInstances)
        {
            effectInstance.Update(deltaTime);

            if (effectInstance.Expired)
            {
                effectInstance.Cancel();
            }
        }

        _effectInstances.RemoveAll(effectInstance => effectInstance.Expired);
    }

    private void UpdateActiveAbilities(float deltaTime)
    {
        foreach (var abilityInstance in _abilityInstances)
        {
            abilityInstance.Update(deltaTime);
        }
    }

    private AbilityInstance GetAbilityInstanceChecked(AbilityHandle handle)
    {
        Debug.Assert(handle.AbilitySystemComponent == this, "Foreign ability handle");

        var abilityInstance = handle.AbilityInstance;

        Debug.Assert(abilityInstance == null || _abilityInstances.Contains(abilityInstance), "Invalid ability handle");

        return abilityInstance;
    }
}

public static class AbilitySystemHelper
{
    private static void AddAttributeValue(this AbilitySystemComponent self, AttributeType attribute, float amount)
    {
        var attributeValue = self.GetAttributeValueObject(attribute);
        if (attributeValue == null)
            return;

        attributeValue.Value += amount;
    }

    public static void AddExperience(this AbilitySystemComponent self, float amount)
    {
        Debug.Assert(!(amount < 0), "Experience amount must be non-negative");

        if (amount > 0)
            self.AddAttributeValue(AttributeType.Experience, amount);
    }

    public static void AddDamage(this AbilitySystemComponent self, float amount)
    {
        Debug.Assert(!(amount < 0), "Damage amount must be non-negative");

        if (amount > 0)
            self.AddAttributeValue(AttributeType.Damage, amount);
    }

    public static void AddHealing(this AbilitySystemComponent self, float amount)
    {
        Debug.Assert(!(amount < 0), "Healing amount must be non-negative");

        if (amount > 0)
            self.AddAttributeValue(AttributeType.Healing, amount);
    }
}
