public class Player : Creature
{
    public float Speed => AbilitySystemComponent.GetAttributeValue(AttributeType.MoveSpeed);
    public float TurnSpeed => AbilitySystemComponent.GetAttributeValue(AttributeType.TurnSpeed);
    public float DamageMin => AbilitySystemComponent.GetAttributeValue(AttributeType.MinDamage);
    public float DamageMax => AbilitySystemComponent.GetAttributeValue(AttributeType.MaxDamage);
    public float FireRate => AbilitySystemComponent.GetAttributeValue(AttributeType.AttackRate);
    public float FireRange => AbilitySystemComponent.GetAttributeValue(AttributeType.AttackRange);
    public float Experience => AbilitySystemComponent.GetAttributeValue(AttributeType.Experience);
    public float Level => AbilitySystemComponent.GetAttributeValue(AttributeType.Level);
}
