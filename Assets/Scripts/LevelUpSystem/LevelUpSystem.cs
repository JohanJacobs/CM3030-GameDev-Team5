using System.Collections;
using System.Collections.Generic;
using System.Xml.XPath;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class LevelUpSystem : MonoBehaviour
{
    [SerializeField] int xpInterval=50;
    [SerializeField] Effect xpEffect;

    Player _player;
    AbilitySystemComponent _abilitySystem;
    
    private void Awake()
    {
        _player = GetComponent<Player>();
        _abilitySystem = GetComponent<AbilitySystemComponent>();
    }

    private void OnEnable()
    {
        Pickup.GlobalPickedUp += Pickup_GlobalPickedUp;
    }

    private void OnDisable()
    {
        Pickup.GlobalPickedUp -= Pickup_GlobalPickedUp;
    }

    private void Pickup_GlobalPickedUp(Pickup pickup, GameObject pickedUpBy)
    {
        if (pickup is ExperienceOrbPickup)
        {
            if (_player.Experience % xpInterval == 0)
            {
                _abilitySystem.ApplyEffectToSelf(xpEffect);                
            }
        }
    }
}
