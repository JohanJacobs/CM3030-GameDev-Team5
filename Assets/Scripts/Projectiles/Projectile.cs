/*

University of London
BsC Computer Science Course
Games Development
Final Assignment - Streets of Fire Game

Group 5 

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

    public GameObject Owner
    {
        get => _owner;
        set => SetOwner(value);
    }

    private GameObject _owner;

    private Collider _collider;
    private Rigidbody _rb;

    protected virtual bool HandleHit(Collider other)
    {
        return true;
    }

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        _rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        if (Lifespan > 0)
        {
            Destroy(gameObject, Lifespan);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == Owner)
            return;

        if (!LayerMask.TestGameObjectLayer(other.gameObject))
            return;

        if (!HandleHit(other))
            return;

        _collider.enabled = false;

        if (DestroyOnHit)
        {
            Destroy(gameObject);
        }
    }

    private void SetOwner(GameObject owner)
    {
        if (_owner == owner)
            return;

        if (_owner)
        {
            Physics.IgnoreCollision(_owner.GetComponent<Collider>(), _collider, false);
        }

        _owner = owner;

        if (_owner)
        {
            Physics.IgnoreCollision(_owner.GetComponent<Collider>(), _collider, true);
        }
    }
}
