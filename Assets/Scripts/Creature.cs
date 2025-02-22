/*

University of London
BsC Computer Science Course
Games Design
Final Assignment - Streets of Fire Game

Group 5 

creature.cs

Class Creature manages a number of creature attributes such as health, state and abilities

*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(AbilitySystemComponent), typeof(HealthComponent))]
public class Creature : MonoBehaviour
{
    public delegate void KillDelegate(Creature creature, Creature victim);
    public delegate void DeathDelegate(Creature creature);
    public delegate void SimpleDelegate();

    public EquipmentAttachmentSlot[] EquipmentAttachmentSlots;

    /// <summary>
    /// This is, roughly, how fast creature will move if driven by animation (depends on animation clip FPS and number of frames).
    /// Used to adjust animation speed based on actual move speed.
    /// </summary>
    public float WalkAnimationMoveSpeed = 2f;

    public bool IsDead => HealthComponent.IsDead;
    public bool IsAlive => HealthComponent.IsAlive;
    public float Health => HealthComponent.Health;
    public float MaxHealth => HealthComponent.MaxHealth;
    public float HealthFraction => HealthComponent.HealthFraction;

    public AbilitySystemComponent AbilitySystemComponent { get; private set; }
    public HealthComponent HealthComponent { get; private set; }

    public event KillDelegate Kill;
    public event DeathDelegate Death;
    public event SimpleDelegate DamageTaken;

    [SerializeField]
    protected bool _autoDestroyOnDeath = true;

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

        DamageTaken?.Invoke();
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
    }

    private void Die()
    {
        OnDeath();

        Death?.Invoke(this);

        if (_autoDestroyOnDeath)
        {
            Destroy(gameObject, 1.5f);
        }
    }

    private void OnOutOfHealth(HealthComponent healthComponent)
    {
        Die();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (EquipmentAttachmentSlots != null)
        {
            var distinctAttachmentSlotCount = EquipmentAttachmentSlots.Select(slot => slot.Slot).Distinct().Count();
            if (distinctAttachmentSlotCount < EquipmentAttachmentSlots.Length)
            {
                Debug.LogWarning("Duplicate attachment slots");
            }
        }
    }
#endif
}
