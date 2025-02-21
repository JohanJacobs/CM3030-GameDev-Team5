using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshPro))]
public class TextPopup : MonoBehaviour
{
    private TextMeshPro _textMesh;
    private Transform _mainCameraTransform;

    float _disspearSpeed = 0.1f;
    float _disappearTimer;
    Color _textColor;
    public void Setup(string text, float timeToLive)
    {
        _disappearTimer = timeToLive;
        _textMesh.SetText(text);
    }
    private void Awake()
    {
        _textMesh = GetComponent<TextMeshPro>();
        _textColor = _textMesh.color;
        _mainCameraTransform = Camera.main.transform;

    }

    private void Update()
    {
        _disappearTimer-= Time.deltaTime;
        if (_disappearTimer < 0f)
        {
            Dissapear();            
        }
    }
    private void LateUpdate()
    {
        FaceTowardsCamera();
    }

    private void Dissapear()
    {
        _textColor.a -= _disspearSpeed*Time.deltaTime;
        _textMesh.color = _textColor;

        // remove the popup if the color invisible
        if (_textColor.a < 0f)
            Destroy(gameObject);
    }

    private void FaceTowardsCamera()
    {
        transform.LookAt(_mainCameraTransform);
        transform.RotateAround(transform.position, transform.up, 180f);
    }
}
