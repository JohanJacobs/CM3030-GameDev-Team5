/*

University of London
BsC Computer Science Course
Games Design
Final Assignment - Streets of Fire Game

Group 5 

AbilityLogic.cs

*/
using System;

public interface IAbilityLogic
{
    void HandleAbilityAdded(AbilityInstance abilityInstance);

    void HandleAbilityRemoved(AbilityInstance abilityInstance);

    void ActivateAbility(AbilityInstance abilityInstance);

    bool CanActivateAbility(AbilityInstance abilityInstance);

    bool CommitAbility(AbilityInstance abilityInstance);

    void EndAbility(AbilityInstance abilityInstance);

    void UpdateAbility(AbilityInstance abilityInstance, float deltaTime);

    void HandleAbilityInputActionPressed(AbilityInstance abilityInstance, InputAction action);

    void HandleAbilityInputActionReleased(AbilityInstance abilityInstance, InputAction action);
}

public abstract class AbilityLogic : IAbilityLogic
{
    public virtual void HandleAbilityAdded(AbilityInstance abilityInstance)
    {
    }

    public virtual void HandleAbilityRemoved(AbilityInstance abilityInstance)
    {
    }

    public virtual void ActivateAbility(AbilityInstance abilityInstance)
    {
    }

    public virtual bool CanActivateAbility(AbilityInstance abilityInstance)
    {
        return true;
    }

    public virtual bool CommitAbility(AbilityInstance abilityInstance)
    {
        return CanActivateAbility(abilityInstance);
    }

    public virtual void EndAbility(AbilityInstance abilityInstance)
    {
    }

    public virtual void UpdateAbility(AbilityInstance abilityInstance, float deltaTime)
    {
    }

    public void HandleAbilityInputActionPressed(AbilityInstance abilityInstance, InputAction action)
    {
    }

    public void HandleAbilityInputActionReleased(AbilityInstance abilityInstance, InputAction action)
    {
    }
}

public sealed class NullAbilityLogic : AbilityLogic
{

}

[AbilityLogicClass]
public class TestAbilityLogic : AbilityLogic
{
    public override void UpdateAbility(AbilityInstance abilityInstance, float deltaTime)
    {
    }
}
