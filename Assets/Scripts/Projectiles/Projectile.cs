/*

University of London
BsC Computer Science Course
Games Development
Final Assignment - Streets of Fire Game

Group 5 

Please refer to the README file for detailled information

Projectile.cs

*/

using System;
using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    public LayerMask LayerMask;
    public float Lifespan = 10;
    public bool DestroyOnHit = true;

    public AbilityInstance AbilityInstance => _abilityInstance;
    public Collider ProjectileCollider => _collider;
    public Rigidbody ProjectileRigidBody => _rb;

    private Collider _collider;
    private Rigidbody _rb;

    private AbilityInstance _abilityInstance;

    private float _lifeTime;

    public void Initialize(AbilityInstance abilityInstance)
    {
        SetAbilityInstance(abilityInstance);
    }

    protected virtual bool HandleHit(Collider other)
    {
        return true;
    }

    protected virtual void OnTriggerEnterImpl(Collider other)
    {
        TryHit(other);
    }

    protected virtual void OnCollisionEnterImpl(Collision collision)
    {
        foreach (var contact in collision.contacts)
        {
            if (TryHit(contact.otherCollider))
                break;
        }
    }

    protected virtual void OnEndOfLife()
    {
    }

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        TickLifeTime();
    }

    private void OnTriggerEnter(Collider other)
    {
        OnTriggerEnterImpl(other);
    }

    private void OnCollisionEnter(Collision collision)
    {
        OnCollisionEnterImpl(collision);
    }

    private void SetAbilityInstance(AbilityInstance abilityInstance)
    {
        if (_abilityInstance == abilityInstance)
            return;

        if (_abilityInstance != null)
        {
            Physics.IgnoreCollision(_abilityInstance.Owner.CreatureCollider, _collider, false);
        }

        _abilityInstance = abilityInstance;

        if (_abilityInstance != null)
        {
            Physics.IgnoreCollision(_abilityInstance.Owner.CreatureCollider, _collider, true);
        }
    }

    private bool TryHit(Collider other)
    {
        if (other.gameObject == _abilityInstance.Owner)
            return false;

        if (!LayerMask.TestGameObjectLayer(other.gameObject))
            return false;

        if (!HandleHit(other))
            return false;

        _collider.enabled = false;

        if (DestroyOnHit)
        {
            Destroy(gameObject);
        }

        return true;
    }

    private void TickLifeTime()
    {
        if (!(Lifespan > 0))
            return;

        _lifeTime += Time.deltaTime;

        if (_lifeTime < Lifespan)
            return;

        OnEndOfLife();

        Destroy(gameObject, 0.05f);
    }
}
