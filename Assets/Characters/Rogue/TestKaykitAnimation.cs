using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestKaykitAnimation : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] bool isDead;
    [SerializeField] bool isHit;

    [Header("DEBUG")]
    [SerializeField] bool fireButtonPressed;
    [SerializeField] Vector3 combinedMovement;

    public void Update()
    {
        fireButtonPressed = Input.GetButton("Fire1");
        animator.SetBool("IsShooting", fireButtonPressed);

        // movement 
        Vector3 movementInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        var movementZ = transform.forward * movementInput.z;
        var movementX = transform.right * movementInput.x;

        combinedMovement = movementZ + movementX;

        if (combinedMovement.magnitude > 1)
        {
            combinedMovement.Normalize();
        }

        // HACK: keep player at Y=0
        transform.Translate(0, -transform.position.y, 0);

        animator.SetFloat("ForwardMovement", movementInput.z);
        animator.SetFloat("RightMovement", movementInput.x);

        animator.SetBool("IsDead", isDead);

        if (isHit)
        {
            animator.SetTrigger("IsHit");
            isHit = false;
        }
    }
}
