public sealed class EffectHandle : GenericInstanceHandle<EffectInstance>
{
    public EffectInstance EffectInstance => Instance;

    /// <summary>
    /// Will be <see langword="true" /> if this handle points to an active effect instance, <see langword="false" /> otherwise
    /// </summary>
    public bool Active
    {
        get
        {
            if (AbilitySystemComponent == null)
                return false;

            // check if EffectInstance is null or expired
            return !(EffectInstance?.Expired ?? true);
        }
    }

    public EffectHandle(AbilitySystemComponent asc, EffectInstance effectInstance)
        : base(asc, effectInstance)
    {
    }
}
