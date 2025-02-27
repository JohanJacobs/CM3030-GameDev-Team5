/*

University of London
BsC Computer Science Course
Games Development
Final Assignment - Streets of Fire Game

Group 5 

Please refer to the README file for detailled information

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
