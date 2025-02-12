using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AbilitySystemComponent : MonoBehaviour
{
    public delegate void SelfDelegate(AbilitySystemComponent asc);

    private static readonly AttributeType[] AllAttributes = Enum.GetValues(typeof(AttributeType)).Cast<AttributeType>().ToArray();

    [SerializeField]
    private AttributeSet[] _grantedAttributeSets;

    private readonly List<AttributeSetInstance> _attributeSets = new List<AttributeSetInstance>();
    private readonly EnumArray<AttributeValue, AttributeType> _attributeValues = new EnumArray<AttributeValue, AttributeType>();
    private readonly EnumArray<AttributeModifierStack, AttributeType> _attributeModifiers = new EnumArray<AttributeModifierStack, AttributeType>();

    private readonly List<EffectInstance> _effects = new List<EffectInstance>();

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
        var index = _attributeSets.FindIndex(asi => asi.Template == attributeSet);
        if (index >= 0)
            return;

        var attributeSetInstance = attributeSet.CreateInstance();

        _attributeSets.Add(attributeSetInstance);

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
        var index = _attributeSets.FindIndex(asi => asi.Template == attributeSet);
        if (index < 0)
            return;

        var attributeSetInstance = _attributeSets[index];

        foreach (var attributeValue in attributeSetInstance.AttributeValues)
        {
            var existingAttributeValue = _attributeValues[attributeValue.Attribute];
            if (existingAttributeValue != attributeValue)
                throw new InvalidOperationException($"Ambiguous attribute {attributeValue.Attribute.GetName()}");

            _attributeValues[attributeValue.Attribute] = null;

            attributeValue.BaseValueChanged -= OnAttributeBaseValueChanged;
        }

        _attributeSets.RemoveAt(index);
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

        if (_effects.Remove(effectInstance))
        {
            effectInstance.Cancel();
        }

        handle.Clear();
    }

    private static void EnsureCanApplyAttributeModifier(AttributeType attribute)
    {
        if (attribute.IsMetaAttribute())
            throw new InvalidOperationException($"Meta attribute {attribute.GetName()} can't have modifiers");
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

        _ready = true;

        Ready?.Invoke(this);
    }

    private void Update()
    {
        var deltaTime = Time.deltaTime;

        UpdateEffects(deltaTime);
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

        _effects.Add(effectInstance);

        var handle = new EffectHandle(this, effectInstance);

        return handle;
    }

    private void UpdateEffects(float deltaTime)
    {
        foreach (var effectInstance in _effects)
        {
            effectInstance.Update(deltaTime);
        }

        _effects.RemoveAll(effectInstance => effectInstance.Expired);
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
