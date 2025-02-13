public sealed class AbilityHandle : GenericInstanceHandle<AbilityInstance>
{
    public AbilityInstance AbilityInstance => Instance;

    public AbilityHandle(AbilitySystemComponent asc, AbilityInstance abilityInstance)
        : base(asc, abilityInstance)
    {
    }
}