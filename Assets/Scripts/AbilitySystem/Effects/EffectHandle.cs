/*

University of London
BsC Computer Science Course
Games Development
Final Assignment - Streets of Fire Game

Group 5 

Please refer to the README file for detailled information

EffectHandle.cs

*/

public sealed class EffectHandle : GenericInstanceHandle<EffectInstance>
{
    public EffectInstance EffectInstance => Instance;
    public EffectContext EffectContext => Instance?.Context;

    /// <summary>
    /// Will be <see langword="true" /> if this handle points to an active effect instance, <see langword="false" /> otherwise
    /// </summary>
    public bool Active
    {
        get
        {
            if (AbilitySystemComponent == null)
                return false;

            // check if EffectInstance exists and active
            return EffectInstance?.Active ?? false;
        }
    }

    public EffectHandle(AbilitySystemComponent asc, EffectInstance effectInstance)
        : base(asc, effectInstance)
    {
    }
}
