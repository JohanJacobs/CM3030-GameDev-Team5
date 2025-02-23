using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *  This class orients text inline with the angle to the player required to see the text in the game world
 */
public class AbilityTextOrientation : MonoBehaviour
{
    Transform _cameraTransform;

    private void Awake()
    {
        _cameraTransform = Camera.main.transform;
    }

    private void LateUpdate()
    {
        UpdateOrientation();
    }

    private void UpdateOrientation()
    {
        transform.rotation = Quaternion.LookRotation(_cameraTransform.forward, _cameraTransform.up);
    }
}