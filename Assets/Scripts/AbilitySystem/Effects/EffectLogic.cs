/*

University of London
BsC Computer Science Course
Games Development
Final Assignment - Streets of Fire Game

Group 5 

Please refer to the README file for detailled information

EffectLogic.cs

*/

public interface IEffectLogic
{
    void ApplyEffect(EffectInstance effectInstance);

    void CancelEffect(EffectInstance effectInstance);

    void UpdateEffect(EffectInstance effectInstance, float deltaTime);
}
