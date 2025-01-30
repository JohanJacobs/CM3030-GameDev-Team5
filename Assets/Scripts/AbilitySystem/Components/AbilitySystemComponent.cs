using UnityEngine;
using System;
using System.Collections.Generic;

public class AbilitySystemComponent : MonoBehaviour
{
    public delegate void SelfDelegate(AbilitySystemComponent asc);

    private class InternalEffect
    {
        public bool IsFinished { get; private set; }

        private readonly Effect _effect;

        private readonly List<AttributeModifierHandle> _attributeModifierHandles = new List<AttributeModifierHandle>();

        private float _timeToFinish;
        private float _timeToNextApplication;

        public InternalEffect(AbilitySystemComponent asc, Effect effect)
        {
            _effect = effect;

            switch (effect.DurationPolicy)
            {
                case EffectDurationPolicy.Instant:
                    IsFinished = true;
                    break;
                case EffectDurationPolicy.Duration:
                    _timeToFinish = effect.Duration;
                    break;
                case EffectDurationPolicy.Infinite:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Apply(AbilitySystemComponent asc)
        {
            _attributeModifierHandles.AddRange(asc.ApplyEffectModifiers(_effect));

            switch (_effect.ApplicationPolicy)
            {
                case EffectApplicationPolicy.Instant:
                    break;
                case EffectApplicationPolicy.Periodic:
                    _timeToNextApplication += _effect.Period;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Cancel(AbilitySystemComponent asc)
        {
            switch (_effect.CancellationPolicy)
            {
                case EffectCancellationPolicy.DoNothing:
                    break;
                case EffectCancellationPolicy.CancelAllModifiers:
                    CancelAllModifiers();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Update(AbilitySystemComponent asc, float deltaTime)
        {
            if (IsFinished)
                return;

            switch (_effect.ApplicationPolicy)
            {
                case EffectApplicationPolicy.Instant:
                    break;
                case EffectApplicationPolicy.Periodic:
                    _timeToNextApplication -= deltaTime;

                    if (!(_timeToNextApplication > 0f))
                    {
                        Apply(asc);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            switch (_effect.DurationPolicy)
            {
                case EffectDurationPolicy.Instant:
                    break;
                case EffectDurationPolicy.Duration:
                    _timeToFinish -= deltaTime;

                    if (!(_timeToFinish > 0f))
                    {
                        IsFinished = true;
                    }
                    break;
                case EffectDurationPolicy.Infinite:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void CancelAllModifiers()
        {
            foreach (var attributeModifierHandle in _attributeModifierHandles)
            {
                attributeModifierHandle.CancelModifier();
            }

            _attributeModifierHandles.Clear();
        }
    }

    public AttributeSet[] DefaultAttributeSets;

    public event SelfDelegate Ready;

    private readonly List<AttributeSet> _attributeSets = new List<AttributeSet>();
    private readonly EnumArray<AttributeValue, AttributeType> _attributeValues = new EnumArray<AttributeValue, AttributeType>();

    private readonly List<InternalEffect> _effects = new List<InternalEffect>();

    private bool _ready = false;

    public void AddAttributeSet(AttributeSet template)
    {
        var index = _attributeSets.FindIndex(attributeSet => attributeSet.Template == template);
        if (index >= 0)
            return;

        var newAttributeSet = template.Spawn();

        foreach (var attribute in newAttributeSet.Attributes)
        {
            var attributeValue = _attributeValues[attribute];
            if (attributeValue == null)
                continue;

            throw new InvalidOperationException($"Ambiguous attribute {attribute.GetName()}");
        }

        foreach (var attribute in newAttributeSet.Attributes)
        {
            var attributeValue = newAttributeSet.GetAttributeValueObject(attribute);

            _attributeValues[attribute] = attributeValue;
        }

        _attributeSets.Add(newAttributeSet);
    }

    public void RemoveAttributeSet(AttributeSet template)
    {
        var index = _attributeSets.FindIndex(attributeSet => attributeSet.Template == template);
        if (index < 0)
            return;

        var oldAttributeSet = _attributeSets[index];

        foreach (var attribute in oldAttributeSet.Attributes)
        {
            _attributeValues[attribute] = null;
        }

        _attributeSets.RemoveAt(index);
    }

    public AttributeModifierHandle ApplyAttributeModifier(AttributeModifier attributeModifer)
    {
        var attributeValue = GetAttributeValueObject(attributeModifer.Attribute);
        if (attributeValue == null)
            throw new InvalidOperationException($"Attribute {attributeModifer.Attribute.GetName()} does not exist");

        return attributeValue.ApplyModifier(attributeModifer);
    }

    public AttributeModifierHandle ApplyAttributeModifier(AttributeType attribute, ScalarModifier modifier, bool post = false, bool permanent = false)
    {
        var attributeValue = GetAttributeValueObject(attribute);
        if (attributeValue == null)
            throw new InvalidOperationException($"Attribute {attribute.GetName()} does not exist");

        return attributeValue.ApplyModifier(modifier, post, permanent);
    }

    public EffectHandle ApplyEffect(Effect effect)
    {
        // TODO: is it OK to apply same effect twice?

        var internalEffect = new InternalEffect(this, effect);

        internalEffect.Apply(this);

        if (internalEffect.IsFinished)
        {
            internalEffect.Cancel(this);
            return null;
        }

        _effects.Add(internalEffect);

        var handle = new EffectHandle(this, effect, internalEffect);

        return handle;
    }

    public void CancelEffect(EffectHandle handle)
    {
        if (handle.AbilitySystemComponent != this)
            throw new ArgumentException("Invalid effect handle");

        if (handle.InternalEffect is InternalEffect internalEffect)
        {
            internalEffect.Cancel(this);

            _effects.Remove(internalEffect);
        }
    }

    public AttributeSet GetAttributeValueContainer(AttributeType attribute)
    {
        return _attributeValues[attribute]?.Owner;
    }

    public AttributeValue GetAttributeValueObject(AttributeType attribute)
    {
        return _attributeValues[attribute];
    }

    public float GetAttributeValue(AttributeType attribute)
    {
        return _attributeValues[attribute]?.Value ?? 0f;
    }

    public void WhenReady(SelfDelegate callback)
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

    private void Start()
    {
        foreach (var attributeSet in DefaultAttributeSets)
        {
            AddAttributeSet(attributeSet);
        }

        Ready?.Invoke(this);

        _ready = true;
    }

    private void Update()
    {
        var deltaTime = Time.deltaTime;

        UpdateEffects(deltaTime);
    }

    private IEnumerable<AttributeModifierHandle> ApplyEffectModifiers(Effect effect)
    {
        foreach (var attributeModifier in effect.Modifiers)
        {
            var attributeModifierHandle = ApplyAttributeModifier(attributeModifier);
            if (attributeModifierHandle != null)
                yield return attributeModifierHandle;
        }
    }

    private void UpdateEffects(float deltaTime)
    {
        for (var i = 0; i < _effects.Count; )
        {
            var internalEffect = _effects[i];

            internalEffect.Update(this, deltaTime);

            if (internalEffect.IsFinished)
            {
                internalEffect.Cancel(this);

                _effects.RemoveAt(i);
            }
            else
            {
                ++i;
            }
        }
    }
}
