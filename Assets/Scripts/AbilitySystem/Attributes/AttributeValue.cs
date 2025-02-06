using UnityEngine;
using System;
using System.Collections.Generic;

public sealed class AttributeValue
{
    public delegate void ModifiedDelegate(AttributeValue attributeValue, float oldValue, float value);

    private class InternalModifier
    {
        public readonly int Index;
        public readonly bool Post;

        public ScalarModifier Modifier;

        public InternalModifier(int index, AttributeModifier attributeModifier)
        {
            Index = index;
            Post = attributeModifier.Post;
            Modifier = ScalarModifier.MakeFromAttributeModifier(attributeModifier);
        }

        public InternalModifier(int index, ScalarModifier modifier, bool post)
        {
            Index = index;
            Post = post;
            Modifier = modifier;
        }

        public void Update(AttributeModifier attributeModifier)
        {
            Modifier.Reset(attributeModifier);
        }

        public void Update(ScalarModifier modifier)
        {
            Modifier.Reset(modifier);
        }
    }

    public AttributeType Attribute { get; }

    public float BaseValue
    {
        get => GetBaseValue();
        set => SetBaseValue(value);
    }

    public float Value => GetValue();

    public AttributeSet Owner => _weakOwner.TryGetTarget(out var owner) ? owner : null;

    /// <summary>
    /// Event fired when value change is caused by modifier
    /// </summary>
    public event ModifiedDelegate Modified;

    private readonly List<InternalModifier> _modifiers = new List<InternalModifier>();
    private readonly WeakReference<AttributeSet> _weakOwner;

    private float _baseValue;
    private float _currentValue;

    private int _internalModifierIndex;

    private bool _dirtyModifiers;
    private bool _dirtyValue;

    private ScalarModifier _permanentModifier = ScalarModifier.MakeIdentity();
    private ScalarModifier _permanentPostModifier = ScalarModifier.MakeIdentity();

    public AttributeValue(AttributeType attribute, AttributeSet owner)
    {
        Attribute = attribute;

        _weakOwner = new WeakReference<AttributeSet>(owner);
    }

    /// <summary>
    /// Apply a modifier which alters resulting value.
    /// Returns a handle to applied modifier that can be later used to update modifier properties.
    /// </summary>
    /// <remarks>
    /// Override modifier directly replaces base value, no handle is returned.<br/>
    /// Permanent modifiers are not stored in modifiers list, no handle is returned.
    /// </remarks>
    /// <param name="attributeModifier">Modifier</param>
    /// <param name="overridePermanent">When specified, overrides attribute modifier's Permanent property</param>
    /// <returns>Applied modifier handle, or null (see remarks)</returns>
    public AttributeModifierHandle ApplyModifier(AttributeModifier attributeModifier, bool? overridePermanent = null)
    {
        Debug.Assert(Attribute == attributeModifier.Attribute);

        var initialValue = Value;

        var handle = ApplyModifierImpl(attributeModifier, overridePermanent);

        var modifiedValue = Value;

        if (!Mathf.Approximately(initialValue, modifiedValue))
        {
            Modified?.Invoke(this, initialValue, modifiedValue);
        }

        return handle;
    }

    /// <summary>
    /// Apply a modifier which alters resulting value.
    /// Returns a handle to applied modifier that can be later used to update modifier properties.
    /// </summary>
    /// <remarks>
    /// Permanent modifiers are not stored in modifiers list, no handle is returned.
    /// </remarks>
    /// <param name="modifier">Modifier</param>
    /// <param name="post">Is this is a post modifier?</param>
    /// <param name="permanent">Is this a permanent modifier?</param>
    /// <returns>Applied modifier handle, or null (see remarks)</returns>
    public AttributeModifierHandle ApplyModifier(ScalarModifier modifier, bool post = false, bool permanent = false)
    {
        var initialValue = Value;

        var handle = ApplyModifierImpl(modifier, post, permanent);

        var modifiedValue = Value;

        if (!Mathf.Approximately(initialValue, modifiedValue))
        {
            Modified?.Invoke(this, initialValue, modifiedValue);
        }

        return handle;
    }

    /// <summary>
    /// Cancel previously applied modifier by handle
    /// </summary>
    /// <param name="handle">Handle</param>
    /// <exception cref="InvalidOperationException"></exception>
    public void CancelModifier(AttributeModifierHandle handle)
    {
        var initialValue = Value;

        CancelModifierImpl(handle);

        var modifiedValue = Value;

        if (!Mathf.Approximately(initialValue, modifiedValue))
        {
            Modified?.Invoke(this, initialValue, modifiedValue);
        }
    }

    /// <summary>
    /// Invalidate stored scalar modifier of previously applied modifier by handle
    /// </summary>
    /// <param name="handle"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public void UpdateModifier(AttributeModifierHandle handle)
    {
        var initialValue = Value;

        UpdateModifierImpl(handle);

        var modifiedValue = Value;

        if (!Mathf.Approximately(initialValue, modifiedValue))
        {
            Modified?.Invoke(this, initialValue, modifiedValue);
        }
    }

    /// <summary>
    /// Resets base value to given value and clears all modifiers
    /// </summary>
    /// <param name="value">New base value</param>
    public void Reset(float value)
    {
        _baseValue = value;
        _currentValue = value;

        _modifiers.Clear();

        _permanentModifier.Clear();
        _permanentPostModifier.Clear();

        _dirtyModifiers = false;
        _dirtyValue = false;
    }

    /// <summary>
    /// Calculates resulting attribute value using <paramref name="baseValue"/> as base value
    /// </summary>
    /// <param name="baseValue">Base value</param>
    /// <returns>Attribute value with all modifiers applied</returns>
    public float Calculate(float baseValue)
    {
        InvalidateModifiers();

        var aggregateModifier = new ScalarModifier(_permanentModifier);
        var aggregatePostModifier = new ScalarModifier(_permanentPostModifier);

        foreach (var internalModifier in _modifiers)
        {
            if (internalModifier.Post)
                aggregatePostModifier.Combine(internalModifier.Modifier);
            else
                aggregateModifier.Combine(internalModifier.Modifier);
        }

        return aggregatePostModifier.Calculate(aggregateModifier.Calculate(baseValue));
    }

    /// <summary>
    /// Compares 2 modifiers
    /// </summary>
    /// <param name="lhs">First modifier</param>
    /// <param name="rhs">Second modifier</param>
    /// <returns>negative if <paramref name="lhs"/> is "less" than <paramref name="rhs"/><br/>0 if both are "equal"<br/>positive if <paramref name="rhs"/> is "less" than <paramref name="lhs"/></returns>
    private static int CompareModifiers(InternalModifier lhs, InternalModifier rhs)
    {
        if (lhs.Post != rhs.Post)
            return lhs.Post ? 1 : -1;

        // TODO: priority?

        return lhs.Index - rhs.Index;
    }

    /// <summary>
    /// Order and combine modifiers if modifiers dirty flag is set
    /// </summary>
    /// <remarks>
    /// Resets modifiers dirty flag
    /// </remarks>
    private void InvalidateModifiers()
    {
        if (!_dirtyModifiers)
            return;

        _modifiers.Sort(CompareModifiers);

        _dirtyModifiers = false;
        _dirtyValue = true;
    }

    /// <summary>
    /// Updates cached value if value dirty flag is set
    /// </summary>
    /// <remarks>
    /// Resets value dirty flag
    /// </remarks>
    private void InvalidateValue()
    {
        if (!_dirtyModifiers && !_dirtyValue)
            return;

        _currentValue = Calculate(_baseValue);

        _dirtyValue = false;
    }

    private float GetBaseValue()
    {
        return _baseValue;
    }

    private void SetBaseValue(float value)
    {
        _baseValue = value;

        _dirtyValue = true;
    }

    private float GetValue()
    {
        InvalidateValue();

        return _currentValue;
    }

    private AttributeModifierHandle ApplyModifierImpl(AttributeModifier attributeModifier, bool? overridePermanent)
    {
        if (attributeModifier.Type == AttributeModifierType.Override)
        {
            SetBaseValue(attributeModifier.Value);

            return null;
        }

        if (overridePermanent ?? attributeModifier.Permanent)
        {
            if (attributeModifier.Post)
                _permanentPostModifier.Combine(ScalarModifier.MakeFromAttributeModifier(attributeModifier));
            else
                _permanentModifier.Combine(ScalarModifier.MakeFromAttributeModifier(attributeModifier));

            _dirtyValue = true;

            return null;
        }

        var internalModifierIndex = ++_internalModifierIndex;

        var internalModifier = new InternalModifier(internalModifierIndex, attributeModifier);

        {
            _modifiers.Add(internalModifier);

            _dirtyModifiers = true;
        }

        var handle = new AttributeModifierHandle(attributeModifier, this, internalModifier);

        return handle;
    }

    private AttributeModifierHandle ApplyModifierImpl(ScalarModifier modifier, bool post, bool permanent)
    {
        if (permanent)
        {
            if (post)
                _permanentPostModifier.Combine(modifier);
            else
                _permanentModifier.Combine(modifier);

            _dirtyValue = true;

            return null;
        }

        var internalModifierIndex = ++_internalModifierIndex;

        var internalModifier = new InternalModifier(internalModifierIndex, modifier, post);

        {
            _modifiers.Add(internalModifier);

            _dirtyModifiers = true;
        }

        var handle = new AttributeModifierHandle(null, this, internalModifier);

        return handle;
    }

    private void CancelModifierImpl(AttributeModifierHandle handle)
    {
        if (handle.AttributeValue != this)
            throw new InvalidOperationException("Invalid attribute modifier handle");

        if (handle.InternalModifier is InternalModifier internalModifier)
        {
            _modifiers.Remove(internalModifier);

            _dirtyModifiers = true;
        }
    }

    private void UpdateModifierImpl(AttributeModifierHandle handle)
    {
        if (handle.AttributeValue != this)
            throw new InvalidOperationException("Invalid attribute modifier handle");

        var modifier = handle.AttributeModifier;
        if (modifier == null)
            return;

        if (handle.InternalModifier is InternalModifier internalModifier)
        {
            internalModifier.Update(modifier);

            _dirtyModifiers = true;
        }
    }
}
