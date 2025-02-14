using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    // NOTE: will be overwritten on each LateUpdate, used to display actual offset
    public Vector3 CameraOffset;

    public float CameraAngle = 60f;
    public float CameraDistance = 10f;

    void LateUpdate()
    {
        var cameraAngleRadians = Mathf.Deg2Rad * CameraAngle;

        var xzLength = CameraDistance * Mathf.Cos(cameraAngleRadians);

        var y = CameraDistance * Mathf.Sin(cameraAngleRadians);
        var xz = Mathf.Sqrt(xzLength);

        CameraOffset = new Vector3(xz, y, xz);

        Camera.main.transform.position = transform.position + CameraOffset;
        Camera.main.transform.LookAt(transform);
    }
}
