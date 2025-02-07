using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperienceOrbAbsorb : MonoBehaviour
{
    [SerializeField] private LayerMask Player;
    [SerializeField] private float absorptionRadius = 4f;
    
    [SerializeField] private float absorptionSpeed = 0f;
    [SerializeField] private float maxAbsorptionSpeed = 13f;
    [SerializeField] private float absorptionAcceleration = 3f;
    private Transform absorptionTarget;

    void Update()
    {
        //Check for the player tag within the OverlapSphere
        if (absorptionTarget == null)
        {
            Collider[] sphereCollisions = Physics.OverlapSphere(transform.position, absorptionRadius, Player);
            if (sphereCollisions.Length > 0) 
            {
                foreach (Collider sphereCollision in sphereCollisions)
                {
                    if (sphereCollision.CompareTag("Player"))
                    {
                        absorptionTarget = sphereCollision.transform;
                        break;
                    }   
                }
            }
        }

        //NON-LINEAR movement towards player
        if (absorptionTarget != null)
        {
            absorptionSpeed += absorptionAcceleration * Time.deltaTime;
            //Set a cap on the max speed of the orb
            absorptionSpeed = Mathf.Min(absorptionSpeed, maxAbsorptionSpeed);

            transform.position = Vector3.MoveTowards(transform.position, absorptionTarget.position, absorptionSpeed * Time.deltaTime);
        }
        // //LINEAR movement towards player
        // if (absorptionTarget != null)
        // {
        //     transform.position = Vector3.MoveTowards(transform.position, absorptionTarget.position, absorptionSpeed * Time.deltaTime);
        // }
    }
}
