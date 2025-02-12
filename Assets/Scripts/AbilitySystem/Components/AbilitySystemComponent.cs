using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class OldAbilitySystemComponent : MonoBehaviour
{
    public delegate void SelfDelegate(OldAbilitySystemComponent asc);

    private class InternalEffect
    {
        public Effect Effect { get; private set; }

        public bool IsFinished => _finished;

        public float TimeLeft => _timeToFinish;

        private readonly List<OldAttributeModifierHandle> _attributeModifierHandles = new List<OldAttributeModifierHandle>();

        private readonly bool _hasDuration;
        private readonly bool _hasPeriod;

        private float _timeToFinish;
        private float _timeToApply;

        private bool _finished;

        public InternalEffect(OldAbilitySystemComponent asc, Effect effect)
        {
            Effect = effect;

            switch (effect.DurationPolicy)
            {
                case EffectDurationPolicy.Instant:
                    _hasDuration = false;
                    _hasPeriod = false;
                    _finished = true;
                    break;
                case EffectDurationPolicy.Duration:
                    _hasDuration = true;
                    _hasPeriod = Effect.HasPeriod;
                    _timeToFinish = effect.Duration;
                    break;
                case EffectDurationPolicy.Infinite:
                    _hasDuration = false;
                    _hasPeriod = Effect.HasPeriod;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Apply(OldAbilitySystemComponent asc)
        {
            if (asc.CanApplyEffect(Effect))
            {
                _attributeModifierHandles.AddRange(asc.ApplyEffectModifiers(Effect));
            }

            if (_hasPeriod)
            {
                _timeToApply += Effect.Period;
            }
        }

        public void Cancel(OldAbilitySystemComponent asc)
        {
            CancelAllModifiers();
        }

        public void Update(OldAbilitySystemComponent asc, float deltaTime)
        {
            if (_finished)
                return;

            if (_hasDuration)
            {
                _timeToFinish -= deltaTime;

                if (!(_timeToFinish > 0f))
                {
                    Finish(asc);
                }
            }

            if (_hasPeriod)
            {
                _timeToApply -= deltaTime;

                if (!(_timeToApply > 0f))
                {
                    Apply(asc);
                }
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

        private void Finish(OldAbilitySystemComponent asc)
        {
            _finished = true;
        }
    }

    public OldAttributeSet[] DefaultAttributeSets;
    public Ability[] DefaultAbilities;

    public event SelfDelegate Ready;

    private readonly List<OldAttributeSet> _attributeSets = new List<OldAttributeSet>();
    private readonly EnumArray<OldAttributeValue, AttributeType> _attributeValues = new EnumArray<OldAttributeValue, AttributeType>();

    private readonly List<InternalEffect> _effects = new List<InternalEffect>();

    private readonly TagContainer _tags = new TagContainer();

    private bool _ready = false;

    public void AddAttributeSet(OldAttributeSet template)
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

    public void RemoveAttributeSet(OldAttributeSet template)
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

    public OldAttributeModifierHandle ApplyAttributeModifier(OldAttributeModifier attributeModifer)
    {
        var attributeValue = GetAttributeValueObject(attributeModifer.Attribute);
        if (attributeValue == null)
            throw new InvalidOperationException($"Attribute {attributeModifer.Attribute.GetName()} does not exist");

        return attributeValue.ApplyModifier(attributeModifer);
    }

    public OldAttributeModifierHandle ApplyAttributeModifier(AttributeType attribute, ScalarModifier modifier, bool post = false, bool permanent = false)
    {
        var attributeValue = GetAttributeValueObject(attribute);
        if (attributeValue == null)
            throw new InvalidOperationException($"Attribute {attribute.GetName()} does not exist");

        return attributeValue.ApplyModifier(modifier, post, permanent);
    }

    public bool CanApplyEffect(Effect effect)
    {
        if (_tags.ContainsAny(effect.BlockTags))
            return false;

        return true;
    }

    public OldEffectHandle ApplyEffect(Effect effect)
    {
        Debug.Assert(effect.IsInstant || effect.IsFinite || effect.IsInfinite);

        if (!CanApplyEffect(effect))
        {
            if (effect.IsInstant)
                return null;

            if (!effect.HasPeriod)
                return null;
        }

        var internalEffect = new InternalEffect(this, effect);

        internalEffect.Apply(this);

        if (internalEffect.IsFinished)
        {
            internalEffect.Cancel(this);
            return null;
        }

        AddActiveEffect(internalEffect);

        var handle = new OldEffectHandle(this, effect, internalEffect);

        return handle;
    }

    public void CancelEffect(OldEffectHandle handle)
    {
        if (handle.AbilitySystemComponent != this)
            throw new ArgumentException("Invalid effect handle");

        if (handle.InternalEffect is InternalEffect internalEffect)
        {
            CancelAndRemoveActiveEffect(internalEffect);
        }
    }

    public float GetEffectTimeLeft(Effect effect)
    {
        var internalEffect = _effects.Find(e => e.Effect == effect);
        if (internalEffect == null)
            return 0f;

        return GetActiveEffectTimeLeft(internalEffect);
    }

    public float GetEffectTimeLeft(OldEffectHandle handle)
    {
        if (handle.AbilitySystemComponent != this)
            throw new ArgumentException("Invalid effect handle");

        if (handle.InternalEffect is InternalEffect internalEffect)
        {
            return GetActiveEffectTimeLeft(internalEffect);
        }

        return 0f;
    }

    public OldAttributeSet GetAttributeValueContainer(AttributeType attribute)
    {
        return _attributeValues[attribute]?.Owner;
    }

    public OldAttributeValue GetAttributeValueObject(AttributeType attribute)
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

        UpdateActiveEffects(deltaTime);
    }

    private void AddActiveEffect(InternalEffect internalEffect)
    {
        _effects.Add(internalEffect);

        _tags.AddRange(internalEffect.Effect.GrantedTags);
    }

    private void RemoveActiveEffect(InternalEffect internalEffect)
    {
        bool removed = _effects.Remove(internalEffect);

        Debug.Assert(removed);

        var tagsToRemove = new HashSet<Tag>(internalEffect.Effect.GrantedTags);

        // we want to remove only those tags that are not granted by other effects
        _effects.ForEach(e => tagsToRemove.ExceptWith(e.Effect.GrantedTags));

        _tags.RemoveRange(tagsToRemove);
    }

    private void CancelAndRemoveActiveEffect(InternalEffect internalEffect)
    {
        internalEffect.Cancel(this);

        RemoveActiveEffect(internalEffect);
    }

    private float GetActiveEffectTimeLeft(InternalEffect internalEffect)
    {
        if (internalEffect.Effect.IsInfinite)
            return float.PositiveInfinity;

        return internalEffect.TimeLeft;
    }

    private void UpdateActiveEffects(float deltaTime)
    {
        for (var i = 0; i < _effects.Count; )
        {
            var internalEffect = _effects[i];

            internalEffect.Update(this, deltaTime);

            if (internalEffect.IsFinished)
            {
                CancelAndRemoveActiveEffect(internalEffect);
            }
            else
            {
                ++i;
            }
        }
    }

    private IEnumerable<OldAttributeModifierHandle> ApplyEffectModifiers(Effect effect)
    {
        return Enumerable.Empty<OldAttributeModifierHandle>();

        // foreach (var attributeModifier in effect.Modifiers)
        // {
        //     var attributeModifierHandle = ApplyAttributeModifier(attributeModifier);
        //     if (attributeModifierHandle != null)
        //         yield return attributeModifierHandle;
        // }
    }
}
