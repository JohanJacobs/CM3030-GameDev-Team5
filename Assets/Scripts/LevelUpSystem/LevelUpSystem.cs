using System.Collections;
using System.Collections.Generic;
using System.Xml.XPath;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class LevelUpSystem : MonoBehaviour
{
    // Delegate that is called when the player reaches a new level
    public delegate void LevelUpDelegate(int LevelNumber);
    public static event LevelUpDelegate GlobalLevelUpEvent;

    // XP Interval at which the player will level up
    [SerializeField] int _xpInterval = 50;

    // Changes to the player when a new level is reached.
    [SerializeField] Effect _xpEffect;

    Player _player;
    AbilitySystemComponent _abilitySystem;

    int _levelnumber = 0;

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

    // When an experience orb is picked up 
    // - Check the XP of the player if we should level up
    // - Notify listeners if the player has leveled up
    private void Pickup_GlobalPickedUp(Pickup pickup, GameObject pickedUpBy)
    {        
        if (pickup is ExperienceOrbPickup && (_player?.Experience % _xpInterval) == 0)
        {
            _levelnumber++;

            _abilitySystem?.ApplyEffectToSelf(_xpEffect);                
            GlobalLevelUpEvent?.Invoke(_levelnumber);
        }
    }
}
