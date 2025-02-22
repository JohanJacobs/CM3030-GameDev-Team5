/*

University of London
BsC Computer Science Course
Games Design
Final Assignment - Streets of Fire Game

Group 5 

ProjectileLinearMovement.cs

*/

using UnityEngine;

[RequireComponent(typeof(Projectile))]
public class ProjectileLinearMovement : MonoBehaviour
{
    public float Speed = 5;

    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        _rb.MovePosition(_rb.position + transform.forward * Speed * Time.deltaTime);
    }
}
