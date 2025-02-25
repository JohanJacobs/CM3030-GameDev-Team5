using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AbilitySystemComponent : MonoBehaviour
{
    public delegate void SelfDelegate(AbilitySystemComponent asc);
    public delegate void AttributeValueMutatorDelegate(AbilitySystemComponent asc, AttributeType attribute, ref float value);
    public delegate void EffectContextDelegate(AbilitySystemComponent asc, Effect effect, EffectContext effectContext);

    private struct ReadyCallback
    {
        public SelfDelegate Delegate;
        public int Priority;
    }

    private static readonly AttributeType[] AllAttributes = Enum.GetValues(typeof(AttributeType)).Cast<AttributeType>().ToArray();

    public IReadOnlyCollection<Tag> Tags => _tags;

    public Creature Owner { get; private set; }

    [SerializeField]
    private AttributeSet[] _grantedAttributeSets;
    [SerializeField]
    private Ability[] _grantedAbilities;

    private readonly List<ReadyCallback> _readyDelegates = new List<ReadyCallback>();

    private readonly List<AttributeSetInstance> _attributeSetInstances = new List<AttributeSetInstance>();
    private readonly EnumArray<AttributeValue, AttributeType> _attributeValues = new EnumArray<AttributeValue, AttributeType>();
    private readonly EnumArray<AttributeModifierStack, AttributeType> _attributeModifiers = new EnumArray<AttributeModifierStack, AttributeType>();

    private readonly List<EffectInstance> _effectInstances = new List<EffectInstance>();

    private readonly List<AbilityInstance> _abilityInstances = new List<AbilityInstance>();

    private readonly TagContainer _tags = new TagContainer();

    // these two store number of delegates subscribed to respective event
    // should generally reduce runtime processing cost in case attribute changes frequently but nothing wants to know of these changes
    private readonly EnumArray<int, AttributeType> _preAttributeModificationDelegateCount = new EnumArray<int, AttributeType>();
    private readonly EnumArray<int, AttributeType> _postAttributeModificationDelegateCount = new EnumArray<int, AttributeType>();

    private bool _ready;

    private event AttributeValueMutatorDelegate PreAttributeModification;
    private event AttributeValueMutatorDelegate PostAttributeModification;

    public void OnReady(SelfDelegate @delegate, int priority = 0)
    {
        if (_ready)
        {
            @delegate.Invoke(this);
        }
        else
        {
            var callback = new ReadyCallback()
            {
                Delegate = @delegate,
                Priority = priority,
            };

            _readyDelegates.Add(callback);
        }
    }

    /// <summary>
    /// Registers delegate to be called before attribute modifier stack is applied. Passed value is attribute's base value.
    /// Doesn't trigger on direct value changes.
    /// </summary>
    /// <remarks>
    /// Only related to modifier stack so it doesn't trigger on direct value changes. Careful with add/remove pairs.
    /// Note also that it's possible to have several callbacks for same attribute.
    /// </remarks>
    /// <param name="attribute"></param>
    /// <param name="delegate"></param>
    public void RegisterPreAttributeModificationCallback(AttributeType attribute, AttributeValueMutatorDelegate @delegate)
    {
        PreAttributeModification += @delegate;

        ref var count = ref _preAttributeModificationDelegateCount[attribute];

        {
            ++count;
        }
    }

    public void UnregisterPreAttributeModificationCallback(AttributeType attribute, AttributeValueMutatorDelegate @delegate)
    {
        PreAttributeModification -= @delegate;

        ref var count = ref _preAttributeModificationDelegateCount[attribute];

        if (count > 0)
        {
            --count;
        }
    }

    /// <summary>
    /// Registers delegate to be called after attribute modifier stack is applied but before storing actual attribute value.
    /// Passed value is attribute's base value with applied modifier stack.
    /// </summary>
    /// <remarks>
    /// Only related to modifier stack so it doesn't trigger on direct value changes. Careful with add/remove pairs.
    /// Note also that it's possible to have several callbacks for same attribute.
    /// </remarks>
    /// <param name="attribute"></param>
    /// <param name="delegate"></param>
    public void RegisterPostAttributeModificationCallback(AttributeType attribute, AttributeValueMutatorDelegate @delegate)
    {
        PostAttributeModification += @delegate;

        ref var count = ref _postAttributeModificationDelegateCount[attribute];

        {
            ++count;
        }
    }

    public void UnregisterPostAttributeModificationCallback(AttributeType attribute, AttributeValueMutatorDelegate @delegate)
    {
        PostAttributeModification -= @delegate;

        ref var count = ref _postAttributeModificationDelegateCount[attribute];

        if (count > 0)
        {
            --count;
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

        attributeValue?.HACK_SetBaseValue(value);
    }

    public float GetAttributeBaseValue(AttributeType attribute)
    {
        var attributeValue = _attributeValues[attribute];

        return attributeValue?.BaseValue ?? 0f;
    }

    public void SetAttributeValue(AttributeType attribute, float value)
    {
        var attributeValue = _attributeValues[attribute];

        attributeValue?.HACK_SetCurrentValue(value);
    }

    public float GetAttributeValue(AttributeType attribute)
    {
        var attributeValue = _attributeValues[attribute];

        return attributeValue?.Value ?? 0f;
    }

    public float GetAttributeValue(AttributeType attribute, out bool attributeExists)
    {
        var attributeValue = _attributeValues[attribute];

        attributeExists = attributeValue != null;

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

    /// <summary>
    /// Applies given attribute modifier.
    /// </summary>
    /// <remarks>
    /// Non-permanent modifiers will still apply if there's no such attribute.
    /// Permanent modifiers are applied to attribute's base value directly, no handle is returned.
    /// </remarks>
    /// <param name="attributeModifier">Attribute modifier to apply</param>
    /// <returns>Handle to attribute modifier instance, or <see langword="null" /></returns>
    public AttributeModifierHandle ApplyAttributeModifier(AttributeModifier attributeModifier)
    {
        if (attributeModifier.Permanent)
        {
            if (attributeModifier.Type == NewAttributeModifierType.Override)
            {
                ApplyPermanentAttributeOverride(attributeModifier.Attribute, attributeModifier.Value);
            }
            else
            {
                ApplyPermanentAttributeModifier(attributeModifier.Attribute, ScalarModifier.MakeFromAttributeModifier(attributeModifier));
            }

            return null;
        }

        var attributeModifierStack = _attributeModifiers[attributeModifier.Attribute];

        var attributeModifierInstance = attributeModifierStack.AddModifier(attributeModifier);

        var handle = new AttributeModifierHandle(this, attributeModifierStack, attributeModifierInstance);

        return handle;
    }

    /// <summary>
    /// Applies scalar modifier to specified attribute.
    /// </summary>
    /// <remarks>
    /// Non-permanent modifiers will still apply if there's no such attribute.
    /// </remarks>
    /// <param name="attribute">Attribute that this modifier is applied to</param>
    /// <param name="scalarModifier">Scalar modifier</param>
    /// <param name="post">Is it a post modifier?</param>
    /// <returns>Handle to attribute modifier instance</returns>
    public AttributeModifierHandle ApplyAttributeModifier(AttributeType attribute, ScalarModifier scalarModifier, bool post)
    {
        var attributeModifierStack = _attributeModifiers[attribute];

        var attributeModifierInstance = attributeModifierStack.AddModifier(scalarModifier, post);

        var handle = new AttributeModifierHandle(this, attributeModifierStack, attributeModifierInstance);

        return handle;
    }

    /// <summary>
    /// Applies override to specified attribute.
    /// </summary>
    /// <remarks>
    /// Non-permanent modifiers will still apply if there's no such attribute.
    /// </remarks>
    /// <param name="attribute">Attribute that this modifier is applied to</param>
    /// <param name="scalarOverride">Override value</param>
    /// <param name="post">Is it a post modifier?</param>
    /// <returns>Handle to attribute modifier instance</returns>
    public AttributeModifierHandle ApplyAttributeOverride(AttributeType attribute, float scalarOverride, bool post)
    {
        var attributeModifierStack = _attributeModifiers[attribute];

        var attributeModifierInstance = attributeModifierStack.AddOverride(scalarOverride, post);

        var handle = new AttributeModifierHandle(this, attributeModifierStack, attributeModifierInstance);

        return handle;
    }

    /// <summary>
    /// Applies permanent attribute modifier which directly modifies base value.
    /// No effect if there's no such attribute.
    /// </summary>
    /// <param name="attribute"></param>
    /// <param name="scalarModifier"></param>
    public void ApplyPermanentAttributeModifier(AttributeType attribute, ScalarModifier scalarModifier)
    {
        var attributeValue = _attributeValues[attribute];

        attributeValue?.HACK_SetBaseValue(scalarModifier.Calculate(attributeValue.BaseValue));
    }

    /// <summary>
    /// Applies permanent attribute override which directly replaces base value.
    /// No effect if there's no such attribute.
    /// </summary>
    /// <param name="attribute">Attribute to apply override to</param>
    /// <param name="scalarOverride">Override value</param>
    public void ApplyPermanentAttributeOverride(AttributeType attribute, float scalarOverride)
    {
        var attributeValue = _attributeValues[attribute];

        attributeValue?.HACK_SetBaseValue(scalarOverride);
    }

    /// <summary>
    /// Cancels attribute modifier instance by its handle. This effectively means that given modifier won't affect the attribute anymore.
    /// </summary>
    /// <param name="handle">Attribute modifier handle</param>
    /// <exception cref="InvalidOperationException"></exception>
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

    /// <summary>
    /// Resets attribute's base value to its current value and clears modifiers.
    /// No effect if there's no such attribute.
    /// </summary>
    /// <param name="attribute">Attribute which modifiers should be collapsed</param>
    public void CollapseAttributeModifiers(AttributeType attribute)
    {
        var attributeValue = _attributeValues[attribute];
        if (attributeValue == null)
            return;

        attributeValue.Reset(attributeValue.Value);

        var attributeModifierStack = _attributeModifiers[attribute];

        attributeModifierStack.Reset();
    }

    #endregion

    #region Effect Context

    public EffectContext CreateEffectContext()
    {
        return new EffectContext(this, this);
    }

    public EffectContext CreateEffectContext(AbilityInstance abilityInstance)
    {
        if (abilityInstance == null)
            throw new ArgumentNullException(nameof(abilityInstance));

        return new EffectContext(this, this, abilityInstance);
    }

    public EffectContext CreateEffectContext(AbilitySystemComponent target)
    {
        if (target == null)
            throw new ArgumentNullException(nameof(target));

        return new EffectContext(this, target);
    }

    public EffectContext CreateEffectContext(AbilitySystemComponent target, AbilityInstance abilityInstance)
    {
        if (target == null)
            throw new ArgumentNullException(nameof(target));
        if (abilityInstance == null)
            throw new ArgumentNullException(nameof(abilityInstance));

        return new EffectContext(this, target, abilityInstance);
    }

    #endregion

    #region Effects

    public bool CanApplyEffect(Effect effect)
    {
        if (effect.HasBlockTags)
        {
            return _tags.ContainsAny(effect.BlockTags);
        }

        return true;
    }

    public EffectHandle ApplyEffectToSelf(Effect effect)
    {
        effect.Validate();

        var context = CreateEffectContext();

        return context.Target.ApplyEffect(effect, context);
    }

    public EffectHandle ApplyEffectToSelf(Effect effect, AbilityInstance abilityInstance)
    {
        effect.Validate();

        var context = CreateEffectContext(abilityInstance);

        return context.Target.ApplyEffect(effect, context);
    }

    public EffectHandle ApplyEffectToTarget(Effect effect, AbilitySystemComponent target)
    {
        effect.Validate();

        var context = CreateEffectContext(target);

        return context.Target.ApplyEffect(effect, context);
    }

    public EffectHandle ApplyEffectToTarget(Effect effect, AbilitySystemComponent target, AbilityInstance abilityInstance)
    {
        effect.Validate();

        var context = CreateEffectContext(target, abilityInstance);

        return context.Target.ApplyEffect(effect, context);
    }

    public EffectHandle ApplyEffectWithContext(Effect effect, EffectContext context)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        effect.Validate();

        if (context.Source != this)
            throw new InvalidOperationException("Invalid effect context source");

        return context.Target.ApplyEffect(effect, context);
    }

    public void CancelEffect(EffectHandle handle)
    {
        if (handle.AbilitySystemComponent != this)
            throw new InvalidOperationException("Invalid effect handle");

        var effectInstance = handle.EffectInstance;
        if (effectInstance == null)
            return;

        // TODO: get rid of Contains call
        if (_effectInstances.Contains(effectInstance))
        {
            RemoveEffectInstanceImpl(effectInstance);
        }

        handle.Clear();
    }

    public EffectInstance FindEffectInstance(Effect effect)
    {
        return _effectInstances.Find(effectInstance => effectInstance.Effect == effect);
    }

    public bool HasActiveEffect(Effect effect)
    {
        var effectInstance = FindEffectInstance(effect);

        return effectInstance?.Active ?? false;
    }

    public float GetActiveEffectTimeRemainingFraction(Effect effect)
    {
        var effectInstance = FindEffectInstance(effect);

        if (effectInstance != null && effectInstance.Active)
            return effectInstance.TimeRemainingFraction;

        return 0f;
    }

    #endregion

    #region Abilities

    public AbilityHandle AddAbility(Ability ability)
    {
        var existingAbilityInstance = _abilityInstances.Find(ai => ai.Ability == ability);
        if (existingAbilityInstance != null)
        {
            return new AbilityHandle(this, existingAbilityInstance);
        }

        if (ability.HasGrantedTags)
        {
            HandleTagsAdded(ability.GrantedTags);
        }

        var abilityInstance = new AbilityInstance(this, ability);

        _abilityInstances.Add(abilityInstance);

        if (ability.HasGrantedTags)
        {
            AddTags(ability.GrantedTags);
        }

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

        RemoveAbilityImpl(abilityInstance);
    }

    public void RemoveAbility(Ability ability)
    {
        var abilityInstance = _abilityInstances.Find(ai => ai.Ability == ability);
        if (abilityInstance == null)
            return;

        RemoveAbilityImpl(abilityInstance);
    }

    public void ActivateAbility(AbilityHandle handle)
    {
        var abilityInstance = GetAbilityInstanceChecked(handle);
        if (abilityInstance == null)
            return;

        if (abilityInstance.Ability.HasBlockTags && _tags.ContainsAny(abilityInstance.Ability.BlockTags))
            return;

        abilityInstance.TryActivate();
    }

    public void AbortAbility(AbilityHandle handle)
    {
        var abilityInstance = GetAbilityInstanceChecked(handle);
        if (abilityInstance == null)
            return;

        abilityInstance.Abort();
    }

    #endregion

    #region Input

    public void OnInputActionPressed(InputAction action)
    {
        foreach (var abilityInstance in _abilityInstances)
        {
            abilityInstance.NotifyInputActionPressed(action);
        }
    }

    public void OnInputActionReleased(InputAction action)
    {
        foreach (var abilityInstance in _abilityInstances)
        {
            abilityInstance.NotifyInputActionReleased(action);
        }
    }

    #endregion

    #region Unity Messages

    private void Awake()
    {
        Owner = GetComponent<Creature>();
        Owner.Death += OnOwnerDeath;
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

        DispatchReady();
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
            abilityInstance.Abort();
            abilityInstance.Destroy();
        }

        foreach (var effectInstance in _effectInstances)
        {
            effectInstance.Cancel();
            effectInstance.Destroy();
        }

        foreach (var attributeModifierInstance in _attributeModifiers)
        {
            attributeModifierInstance.Reset();
        }

        _attributeSetInstances.Clear();
        _attributeValues.Fill(null);
        _attributeModifiers.Fill(null);
        _effectInstances.Clear();
        _abilityInstances.Clear();
        _tags.Clear();

        Owner = null;
    }

    #endregion

    #region Private

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
        var attribute = attributeValue.Attribute;

        float value = attributeValue.BaseValue;

        if (!attribute.IsMetaAttribute() && _preAttributeModificationDelegateCount[attribute] > 0)
        {
            PreAttributeModification?.Invoke(this, attribute, ref value);
        }

        value = attributeModifierStack.Calculate(attributeValue.BaseValue);

        if (!attribute.IsMetaAttribute() && _postAttributeModificationDelegateCount[attribute] > 0)
        {
            PostAttributeModification?.Invoke(this, attribute, ref value);
        }

        attributeValue.HACK_SetCurrentValue(value);
    }

    private EffectHandle ApplyEffect(Effect effect, EffectContext context)
    {
        bool canApplyEffect = CanApplyEffect(effect);
        bool hasDuration;
        bool hasPeriod;

        switch (effect.DurationPolicy)
        {
            case EffectDurationPolicy.Instant:
                if (!canApplyEffect)
                    return null;
                break;
            case EffectDurationPolicy.Duration:
                hasDuration = effect.Duration.Calculate(context) > 0;
                if (!hasDuration)
                    return null;
                hasPeriod = effect.Period.Calculate(context) > 0;
                if (!canApplyEffect && !hasPeriod)
                    return null;
                break;
            case EffectDurationPolicy.Infinite:
                hasPeriod = effect.Period.Calculate(context) > 0;
                if (!canApplyEffect && !hasPeriod)
                    return null;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        // TODO: should this also trigger when periodic effect is applied?
        if (effect.HasGrantedTags)
        {
            HandleTagsAdded(effect.GrantedTags);
        }

        var effectInstance = effect.CreateEffectInstance(effect, context);

        if (effect.Instant)
        {
            effectInstance.Apply();
            effectInstance.Cancel();
            effectInstance.Destroy();
            return null;
        }

        if (canApplyEffect)
        {
            effectInstance.Apply();
        }

        _effectInstances.Add(effectInstance);

        if (effect.HasGrantedTags)
        {
            AddTags(effect.GrantedTags);
        }

        var handle = new EffectHandle(this, effectInstance);

        return handle;
    }

    private void RemoveEffectInstanceImpl(EffectInstance effectInstance)
    {
        effectInstance.Cancel();

        _effectInstances.Remove(effectInstance);

        if (effectInstance.Effect.HasGrantedTags)
        {
            RemoveTags(effectInstance.Effect.GrantedTags);
        }

        effectInstance.Destroy();
    }

    private void RemoveEffectInstanceImpl(EffectInstance effectInstance, int index)
    {
        Debug.Assert(_effectInstances[index] == effectInstance);

        effectInstance.Cancel();

        _effectInstances.RemoveAt(index);

        if (effectInstance.Effect.HasGrantedTags)
        {
            RemoveTags(effectInstance.Effect.GrantedTags);
        }

        effectInstance.Destroy();
    }

    private void HandleTagsAdded(IEnumerable<Tag> tags)
    {
        var addedTagsContainer = new TagContainer(tags);

        var abilitiesToAbort = _abilityInstances
            .Where(abilityInstance => abilityInstance.Ability.HasCancelTags)
            .Where(abilityInstance => addedTagsContainer.ContainsAny(abilityInstance.Ability.CancelTags));
        var effectsToCancel = _effectInstances
            .Where(effectInstance => effectInstance.Effect.HasCancelTags)
            .Where(effectInstance => addedTagsContainer.ContainsAny(effectInstance.Effect.CancelTags));

        foreach (var abilityInstance in abilitiesToAbort)
        {
            abilityInstance.Abort();
        }

        foreach (var effectInstance in effectsToCancel)
        {
            RemoveEffectInstanceImpl(effectInstance);
        }
    }

    private void AddTags(IEnumerable<Tag> tags)
    {
        _tags.AddRange(tags);
    }

    private void RemoveTags(IEnumerable<Tag> tags)
    {
        // TODO: keep and update these lists separately?
        var abilitiesGrantedTags = _abilityInstances
            .Where(abilityInstance => abilityInstance.Ability.HasGrantedTags)
            .SelectMany(abilityInstance => abilityInstance.Ability.GrantedTags);
        var activeEffectsGrantedTags = _effectInstances
            .Where(effectInstance => effectInstance.Active)
            .Where(effectInstance => effectInstance.Effect.HasGrantedTags)
            .SelectMany(effectInstance => effectInstance.Effect.GrantedTags);

        var removedTagsContainer = new TagContainer(tags);

        removedTagsContainer.RemoveRange(abilitiesGrantedTags);
        removedTagsContainer.RemoveRange(activeEffectsGrantedTags);

        _tags.RemoveRange(removedTagsContainer);
    }

    private void UpdateActiveEffects(float deltaTime)
    {
        for (var i = 0; i < _effectInstances.Count; )
        {
            var effectInstance = _effectInstances[i];

            effectInstance.Update(deltaTime);

            if (effectInstance.Inactive)
            {
                RemoveEffectInstanceImpl(effectInstance, i);
            }
            else
            {
                ++i;
            }
        }
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

    private void RemoveAbilityImpl(AbilityInstance abilityInstance)
    {
        abilityInstance.Abort();

        _abilityInstances.Remove(abilityInstance);

        if (abilityInstance.Ability.HasGrantedTags)
        {
            RemoveTags(abilityInstance.Ability.GrantedTags);
        }

        abilityInstance.NotifyRemoved();
        abilityInstance.Destroy();
    }

    private void RemoveAbilityImpl(AbilityInstance abilityInstance, int index)
    {
        Debug.Assert(_abilityInstances[index] == abilityInstance);

        abilityInstance.Abort();

        _abilityInstances.RemoveAt(index);

        if (abilityInstance.Ability.HasGrantedTags)
        {
            RemoveTags(abilityInstance.Ability.GrantedTags);
        }

        abilityInstance.NotifyRemoved();
        abilityInstance.Destroy();
    }

    private void DispatchReady()
    {
        Debug.Assert(!_ready);

        _ready = true;

        _readyDelegates.Sort((lhs, rhs) => rhs.Priority - lhs.Priority);
        _readyDelegates.ForEach(@delegate => @delegate.Delegate.Invoke(this));
        _readyDelegates.Clear();
    }

    private void OnOwnerDeath(Creature creature)
    {
        AddTags(new []{ GameData.Instance.Tags.ConditionDead });

        foreach (var effectInstance in _effectInstances.ToArray())
        {
            RemoveEffectInstanceImpl(effectInstance);
        }

        foreach (var abilityInstance in _abilityInstances.ToArray())
        {
            RemoveAbilityImpl(abilityInstance);
        }
    }

    #endregion
}