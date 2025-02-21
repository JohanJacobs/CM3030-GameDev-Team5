using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    private TextMeshPro textMesh;

    private void Awake()
    {
        textMesh = transform.GetComponent<TextMeshPro>();
    }
    
    public void Setup(string popupMsg)
    {
        textMesh.SetText(popupMsg);
    }
}
