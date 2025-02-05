using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Creature : MonoBehaviour
{
    public delegate void KillDelegate(Creature creature, Creature victim);
    public delegate void DeathDelegate(Creature creature);
    public delegate void ReceiveDamangeDelegate();

    public float MaxHealth = 10f;

    public bool IsDead => _dead;
    public bool IsAlive => !_dead;
    public float Health => _health;
    public float HealthFraction => Health / MaxHealth;

    public event KillDelegate Kill;
    public event DeathDelegate Death;
    public event ReceiveDamangeDelegate ReceiveDamanage;

    private float _health;
    private bool _dead = false;

    void Awake()
    {
        _health = MaxHealth;
    }

    public void TakeDamage(GameObject causer, Vector3 origin, float amount)
    {
        if (IsDead)
            return;

        if (!(amount > 0))
            throw new ArgumentOutOfRangeException(nameof(amount), amount, "Damage amount must be positive");

        if (_health > amount)
        {
            _health -= amount;

            OnDamageTaken(causer, origin, amount);
        }
        else
        {
            _health = 0;

            Die();
        }
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
        _dead = true;

        OnDeath();

        Death?.Invoke(this);

        /// only destroy monster gameobjects.
        if (!gameObject.CompareTag("Player"))            
            GameObject.Destroy(gameObject, 0.2f);

    }
}
