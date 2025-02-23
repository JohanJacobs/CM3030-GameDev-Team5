using System.Text;
using TMPro;
using UnityEngine;

/**
 *  This class orients text perpendicular to camera (essentially a billboard).
 *  Contains methods to further update text for concrete pickup.
 */
[RequireComponent(typeof(TextMeshPro))]
public class PickupText : MonoBehaviour
{
    private Transform _cameraTransform;

    private void Awake()
    {
        _cameraTransform = Camera.main.transform;
    }

    private void Start()
    {
        var pickup = GetComponentInParent<Pickup>();
        var tmp = GetComponent<TextMeshPro>();

        UpdatePickupText(pickup, tmp);
    }

    private void LateUpdate()
    {
        UpdateOrientation();
    }

    protected virtual void UpdatePickupText(Pickup pickup, TextMeshPro tmp)
    {
    }

    protected void SetText(TextMeshPro tmp, string header, string description)
    {
        var sb = new StringBuilder();

        sb.Append("<uppercase>");
        sb.Append(header.Trim());
        sb.Append("</uppercase>");

        if (!string.IsNullOrWhiteSpace(description))
        {
            sb.AppendLine();
            sb.Append("<size=60%>");
            sb.Append(description.Trim());
            sb.Append("</size>");
        }

        tmp.text = sb.ToString();
    }

    private void UpdateOrientation()
    {
        transform.rotation = Quaternion.LookRotation(_cameraTransform.forward, _cameraTransform.up);
    }
}