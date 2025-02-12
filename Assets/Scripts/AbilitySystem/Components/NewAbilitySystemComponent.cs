using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class NewAbilitySystemComponent : MonoBehaviour
{
    public delegate void SelfDelegate(NewAbilitySystemComponent asc);

    private static readonly AttributeType[] AllAttributes = Enum.GetValues(typeof(AttributeType)).Cast<AttributeType>().ToArray();

    [SerializeField]
    private NewAttributeSet[] _grantedAttributeSets;

    private readonly List<NewAttributeSetInstance> _attributeSets = new List<NewAttributeSetInstance>();
    private readonly EnumArray<NewAttributeValue, AttributeType> _attributeValues = new EnumArray<NewAttributeValue, AttributeType>();
    private readonly EnumArray<NewAttributeModifierStack, AttributeType> _attributeModifiers = new EnumArray<NewAttributeModifierStack, AttributeType>();

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

    public void AddAttributeSet(NewAttributeSet attributeSet)
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

    public void RemoveAttributeSet(NewAttributeSet attributeSet)
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

    public NewAttributeValue GetAttributeValueObject(AttributeType attribute)
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

    public NewAttributeModifierHandle ApplyAttributeModifier(NewAttributeModifier attributeModifier)
    {
        var attributeModifierStack = _attributeModifiers[attributeModifier.Attribute];

        var attributeModifierInstance = attributeModifierStack.AddModifier(attributeModifier);

        var handle = new NewAttributeModifierHandle(this, attributeModifierStack, attributeModifierInstance);

        return handle;
    }

    public NewAttributeModifierHandle ApplyAttributeModifier(AttributeType attribute, ScalarModifier scalarModifier, bool post)
    {
        var attributeModifierStack = _attributeModifiers[attribute];

        var attributeModifierInstance = attributeModifierStack.AddModifier(scalarModifier, post);

        var handle = new NewAttributeModifierHandle(this, attributeModifierStack, attributeModifierInstance);

        return handle;
    }

    public void CancelAttributeModifier(NewAttributeModifierHandle handle)
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

    public NewEffectHandle ApplyEffectToSelf(Effect effect)
    {
        Debug.Assert(effect.IsInstant || effect.IsFinite || effect.IsInfinite);

        var context = new EffectContext(this, this);

        return context.Target.ApplyEffect(effect, context);
    }

    public NewEffectHandle ApplyEffectToTarget(Effect effect, NewAbilitySystemComponent target)
    {
        Debug.Assert(effect.IsInstant || effect.IsFinite || effect.IsInfinite);

        var context = new EffectContext(this, target);

        return context.Target.ApplyEffect(effect, context);
    }

    public void CancelEffect(NewEffectHandle handle)
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

    private void Start()
    {
        foreach (var attribute in AllAttributes)
        {
            var stack = new NewAttributeModifierStack(attribute);

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

    private void OnAttributeBaseValueChanged(NewAttributeValue attributeValue, float oldValue, float newValue)
    {
        UpdateAttributeValue(attributeValue);
    }

    private void OnAttributeModifierStackChanged(NewAttributeModifierStack attributeModifierStack)
    {
        var attributeValue = _attributeValues[attributeModifierStack.Attribute];
        if (attributeValue == null)
            return;

        UpdateAttributeValue(attributeValue, attributeModifierStack);
    }

    private void UpdateAttributeValue(NewAttributeValue attributeValue)
    {
        var attributeModifierStack = _attributeModifiers[attributeValue.Attribute];

        UpdateAttributeValue(attributeValue, attributeModifierStack);
    }

    private void UpdateAttributeValue(NewAttributeValue attributeValue, NewAttributeModifierStack attributeModifierStack)
    {
        attributeValue.Value = attributeModifierStack.Calculate(attributeValue.BaseValue);
    }

    private NewEffectHandle ApplyEffect(Effect effect, EffectContext context)
    {
        var effectInstance = new EffectInstance(effect, context);

        effectInstance.Apply();

        if (effectInstance.Expired)
        {
            effectInstance.Cancel();

            return null;
        }

        _effects.Add(effectInstance);

        var handle = new NewEffectHandle(this, effectInstance);

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
