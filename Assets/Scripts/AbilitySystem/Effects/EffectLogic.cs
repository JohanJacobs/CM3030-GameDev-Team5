public interface IEffectLogic
{
    void ApplyEffect(EffectInstance effectInstance);

    void CancelEffect(EffectInstance effectInstance);

    void UpdateEffect(EffectInstance effectInstance, float deltaTime);
}