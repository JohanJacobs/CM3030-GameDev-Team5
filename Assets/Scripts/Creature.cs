/*

University of London
BsC Computer Science Course
Games Development
Final Assignment - Streets of Fire Game

Group 5 

Please refer to the README file for detailled information

Creature.cs

Class Creature manages a number of creature attributes such as health, state and abilities

*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static UnityEngine.UI.Image;

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
    public Collider CreatureCollider { get; private set; }

    public event KillDelegate Kill;
    public event DeathDelegate Death;
    public event SimpleDelegate DamageTaken;

    [SerializeField]
    protected bool _autoDestroyOnDeath = true;

    protected virtual void Awake()
    {
        AbilitySystemComponent = GetComponent<AbilitySystemComponent>();

        HealthComponent = GetComponent<HealthComponent>();
        HealthComponent.OutOfHealth += OnOutOfHealth;

        CreatureCollider = GetCreatureCollider();
    }

    public void NotifyDamageTaken(in DamageEvent damageEvent)
    {
        OnDamageTaken(damageEvent.Causer, damageEvent.Causer.transform.position, damageEvent.Amount);

        DamageTaken?.Invoke();
    }

    public void NotifyDamageDealt(in DamageEvent damageEvent)
    {
        if (damageEvent.Lethal)
        {
            OnKill(damageEvent.Target.Owner);

            Kill?.Invoke(this, damageEvent.Target.Owner);
        }
    }

    public bool TryFindEquipmentAttachmentSlot(EquipmentSlot equipmentSlot, out EquipmentAttachmentSlot attachmentSlot)
    {
        attachmentSlot = EquipmentAttachmentSlots.FirstOrDefault(slot => slot.Slot == equipmentSlot);

        return attachmentSlot != null;
    }

    public void Suicide()
    {
        var effectContext = AbilitySystemComponent.CreateEffectContext();

        effectContext.SetValue(DamageEffect.SelfDestruct, true);

        AbilitySystemComponent.ApplyEffectWithContext(GameData.Instance.DefaultDamageEffect, effectContext);
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

    protected virtual Collider GetCreatureCollider()
    {
        return GetComponent<Collider>();
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

            if (EquipmentAttachmentSlots.Any(slot => slot.Slot == EquipmentSlot.Undefined))
            {
                Debug.LogWarning("Equipment slot Undefined must not be used for attachments");
            }
        }
    }
#endif
}
