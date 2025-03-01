using System;
using System.Linq;
using UnityEngine;

public struct DamageEvent
{
    /// <summary>
    /// Ability that deals this damage. Can be null.
    /// </summary>
    public AbilityInstance AbilityInstance;
    public AbilitySystemComponent Source;
    public AbilitySystemComponent Target;
    /// <summary>
    /// Object that actually caused this damage, e.g. projectile
    /// </summary>
    public GameObject Causer;
    public float Amount;
    public bool Critical;
    public bool Lethal;
}

[RequireComponent(typeof(GameController))]
public class DamageSystem : MonoBehaviour
{
    public static DamageSystem ActiveInstance => _activeInstance;

    private const int MaxDamageEvents = 256;

    private static DamageSystem _activeInstance;

    private readonly DamageEvent[] _damageEvents = new DamageEvent[MaxDamageEvents];

    private int _damageEventCount;

    public void PostDamageEvent(in DamageEvent damageEvent)
    {
        Debug.Assert(_damageEventCount < MaxDamageEvents);

        _damageEvents[_damageEventCount] = damageEvent;

        ++_damageEventCount;
    }

    private void Awake()
    {
        // set static instance
        {
            Debug.Assert(_activeInstance == null);

            _activeInstance = this;
        }
    }

    private void LateUpdate()
    {
        for (var i = 0; i < _damageEventCount; ++i)
        {
            ref var damageEvent = ref _damageEvents[i];

            ProcessDamageEvent(ref damageEvent);
        }

        Array.Clear(_damageEvents, 0, _damageEventCount);

        _damageEventCount = 0;
    }

    private void OnDestroy()
    {
        {
            Debug.Assert(_activeInstance == this);

            _activeInstance = null;
        }
    }

    private void ProcessDamageEvent(ref DamageEvent damageEvent)
    {
        if (damageEvent.Target.Owner.IsDead)
            return;

        damageEvent.Target.AddDamage(damageEvent.Amount);

        damageEvent.Lethal = damageEvent.Target.Owner.IsDead;

        if (damageEvent.Source != damageEvent.Target)
        {
            damageEvent.Source.Owner.NotifyDamageDealt(damageEvent);
        }

        damageEvent.Target.Owner.NotifyDamageTaken(damageEvent);
    }
}