using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour
{
    public delegate void KillDelegate(Creature creature, Creature victim);
    public delegate void DeathDelegate(Creature creature);
    public delegate void ReceiveDamangeDelegate();

    public bool IsDead => HealthComponent.IsDead;
    public bool IsAlive => HealthComponent.IsAlive;
    public float Health => HealthComponent.Health;
    public float MaxHealth => HealthComponent.MaxHealth;
    public float HealthFraction => HealthComponent.HealthFraction;

    public AbilitySystemComponent AbilitySystemComponent { get; private set; }
    public HealthComponent HealthComponent { get; private set; }

    public event KillDelegate Kill;
    public event DeathDelegate Death;
    public event ReceiveDamangeDelegate ReceiveDamanage;

    void Awake()
    {
        AbilitySystemComponent = GetComponent<AbilitySystemComponent>();

        HealthComponent = GetComponent<HealthComponent>();
        HealthComponent.OutOfHealth += OnOutOfHealth;
    }

    public void TakeDamage(GameObject causer, Vector3 origin, float amount)
    {
        if (IsDead)
            return;

        AbilitySystemComponent.AddDamage(amount);

        OnDamageTaken(causer, origin, amount);
    }

    public void DealDamage(Creature victim, Vector3 origin, float amount)
    {
        if (victim == null)
            return;

        if (victim.IsDead)
            return;

        victim.TakeDamage(GetDamageCauser(), origin, amount);

        if (victim.IsDead)
        {
            OnKill(victim);

            Kill?.Invoke(this, victim);
        }
    }

    protected virtual GameObject GetDamageCauser()
    {
        return gameObject;
    }

    protected virtual void OnKill(Creature victim)
    {

    }

    protected virtual void OnDeath()
    {

    }

    protected virtual void OnDamageTaken(GameObject causer, Vector3 origin, float amount)
    {
        ReceiveDamanage?.Invoke();
    }

    private void Die()
    {
        OnDeath();

        Death?.Invoke(this);

        // only destroy non player objects
        if (!gameObject.CompareTag("Player"))
            GameObject.Destroy(gameObject, 0.2f);
    }

    private void OnOutOfHealth(HealthComponent healthComponent)
    {
        Die();
    }
}
