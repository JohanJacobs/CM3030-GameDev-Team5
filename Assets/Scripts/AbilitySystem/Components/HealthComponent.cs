using System;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    public delegate void SelfDelegate(HealthComponent component);
    public delegate void HealthChangedDelegate(HealthComponent component, float oldHealth, float newHealth);

    public float Health => _healthAttribute.Value;

    public float MaxHealth => _maxHealthAttribute.Value;

    public float HealthFraction
    {
        get
        {
            var health = Health;
            var maxHealth = MaxHealth;

            if (maxHealth > 0)
                return Mathf.Clamp(health, 0f, maxHealth) / maxHealth;

            return 0f;
        }
    }

    public bool IsAlive => Health > 0f;

    public bool IsDead => !IsAlive;

    public event SelfDelegate OutOfHealth;
    public event HealthChangedDelegate HealthChanged;

    private AttributeValue _healthAttribute;
    private AttributeValue _maxHealthAttribute;
    private AttributeValue _healthRegenerationAttribute;
    private AttributeValue _damageAttribute;
    private AttributeValue _healingAttribute;

    private void Awake()
    {
        var asc = GetComponent<AbilitySystemComponent>();

        asc.OnReady(OnAbilitySystemReady, 10);
    }

    private void Start()
    {
    }

    private void LateUpdate()
    {
        UpdateHealthRegeneration();
    }

    private void OnAbilitySystemReady(AbilitySystemComponent asc)
    {
        _healthAttribute = asc.GetAttributeValueObject(AttributeType.Health);
        _maxHealthAttribute = asc.GetAttributeValueObject(AttributeType.MaxHealth);
        _healthRegenerationAttribute = asc.GetAttributeValueObject(AttributeType.HealthRegeneration);
        _damageAttribute = asc.GetAttributeValueObject(AttributeType.Damage);
        _healingAttribute = asc.GetAttributeValueObject(AttributeType.Healing);

        // TODO: reset max health if it's less than 1 ?

        _healthAttribute.BaseValue = _maxHealthAttribute.Value;

        _damageAttribute.ValueChanged += OnDamageModified;
        _healingAttribute.ValueChanged += OnHealingModified;

        {
            HealthChanged?.Invoke(this, Health, Health);
        }
    }

    private void OnDamageModified(AttributeValue attributeValue, float oldValue, float value)
    {
        Debug.Assert(!(value < 0f), "Damage must be non-negative");

        var maxHealth = Mathf.Max(MaxHealth, 1f);
        var health = Mathf.Clamp(Health, 0f, maxHealth);

        var initialHealth = health;

        bool wasDead = !(health > 0);

        if (!wasDead)
        {
            health = Mathf.Clamp(health - value, 0f, maxHealth);
        }

        bool isDead = !(health > 0);

        _healthAttribute.Reset(health);
        _damageAttribute.Reset(0f);

        if (initialHealth > health)
        {
            HealthChanged?.Invoke(this, initialHealth, health);
        }

        if (wasDead != isDead)
        {
            OutOfHealth?.Invoke(this);
        }
    }

    private void OnHealingModified(AttributeValue attributeValue, float oldValue, float value)
    {
        Debug.Assert(!(value < 0f), "Healing must be non-negative");

        var maxHealth = Mathf.Max(MaxHealth, 1f);
        var health = Mathf.Clamp(Health, 0f, maxHealth);

        var initialHealth = health;

        bool wasDead = !(health > 0);

        if (!wasDead)
        {
            health = Mathf.Clamp(health + value, 0f, maxHealth);
        }

        _healthAttribute.Reset(health);
        _healingAttribute.Reset(0f);

        if (initialHealth < health)
        {
            HealthChanged?.Invoke(this, initialHealth, health);
        }
    }

    private void UpdateHealthRegeneration()
    {
        var regeneration = _healthRegenerationAttribute?.Value ?? 0f;

        Debug.Assert(!(regeneration < 0f), "Health regeneration must be non-negative");

        if (regeneration > 0f)
        {
            _healingAttribute.Value += regeneration * Time.deltaTime;
        }
    }
}
