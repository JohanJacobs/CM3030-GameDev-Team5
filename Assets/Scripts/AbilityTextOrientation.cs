using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *  This class orients text inline with the angle to the player required to see the text in the game world
 */
public class AbilityTextOrientation : MonoBehaviour
{
    Transform _playerTransform;
    Transform _cameraTransform;
    private void Awake()
    {
        _playerTransform = FindObjectOfType<Player>().transform;
        _cameraTransform = Camera.main.transform;
    }


    private void LateUpdate()
    {
        UpdateOrientation();
    }


    private void UpdateOrientation()
    {
        var vector_from_player_to_camera = _cameraTransform.position - _playerTransform.position;

        var look_at_position = transform.position - vector_from_player_to_camera;
        transform.LookAt(look_at_position);        
    }
}
