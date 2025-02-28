using UnityEngine;

public struct DamageEvent
{
    public AbilityInstance AbilityInstance;
    public GameObject Instigator;
    public GameObject Causer;
    public GameObject Victim;
    public float Damage;
    public bool Critical;
    public Vector3 Origin;
}