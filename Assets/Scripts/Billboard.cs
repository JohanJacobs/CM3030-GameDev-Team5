using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform _mainCameraTransform;

    private void Awake()
    {
        _mainCameraTransform = Camera.main.transform;
    }

    private void LateUpdate()
    {
        transform.rotation = Quaternion.LookRotation(_mainCameraTransform.forward, _mainCameraTransform.up);
    }
}