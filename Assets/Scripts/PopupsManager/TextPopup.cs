using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshPro))]
public class TextPopup : MonoBehaviour
{
    private TextMeshPro _textMesh;
    private Transform _mainCameraTransform;

    private Transform _player;
    
    // disappear and lifetime
    float _disappearSpeed = 1f;
    float _disappearTimer;
    Color _textColor;

    // movement 
    float _moveUpSpeed = 4f;
    public void Setup(string text, Transform playerTransform, float timeToLive)
    {
        _disappearTimer = timeToLive;
        _textMesh.SetText(text);
        _player = playerTransform;
    }
    private void Awake()
    {
        _textMesh = GetComponent<TextMeshPro>();
        _textColor = _textMesh.color;
        _mainCameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        // update the Visual to slowly fade
        _disappearTimer-= Time.deltaTime;
        if (_disappearTimer < 0f)
        {
            Disappear();            
        }

        // move the popup in the positive y direction
        MovePopup();
    }
    private void LateUpdate()
    {
        FaceTowardsCamera();
    }

    private void Disappear()
    {
        // reduce alpha value of popup
        _textColor.a -= _disappearSpeed*Time.deltaTime;
        _textMesh.color = _textColor;

        // remove the popup if the color invisible
        if (_textColor.a <= 0.2f)
            Destroy(gameObject);
    }

    private void MovePopup()
    {
        // move the popup in the game world
        transform.position += new Vector3(0f, _moveUpSpeed * Time.deltaTime);
    }

    private void FaceTowardsCamera()
    {
        // direction from the player to the camera
        var vec_from_player_to_screen = (_mainCameraTransform.position - _player.position);
        var dist = vec_from_player_to_screen.magnitude;
        var norm_vec = vec_from_player_to_screen.normalized;

        // the point the pickup should be pointing to get the correct orientation
        var text_look_point = transform.position - norm_vec * dist;
        
        transform.LookAt(text_look_point);
    }
}
