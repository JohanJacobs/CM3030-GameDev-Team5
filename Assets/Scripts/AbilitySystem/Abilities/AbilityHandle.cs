/*

University of London
BsC Computer Science Course
Games Design
Final Assignment - Streets of Fire Game

Group 5 

AbilityHandle.cs

*/

public sealed class AbilityHandle : GenericInstanceHandle<AbilityInstance>

{
    public AbilityInstance AbilityInstance => Instance;

    public AbilityHandle(AbilitySystemComponent asc, AbilityInstance abilityInstance)
        : base(asc, abilityInstance)
    {
    }
}
