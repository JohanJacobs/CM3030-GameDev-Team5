using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    private TextMeshProUGUI textMesh;

    private void Awake()
    {
        textMesh = transform.GetComponent<TextMeshProUGUI>();
        Debug.Log(textMesh);
    } 
    
    public void Setup(string popupMsg)
    {
        textMesh.SetText(popupMsg);
    }
}
