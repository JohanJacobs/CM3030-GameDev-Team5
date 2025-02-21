// using UnityEngine;
// using TMPro;

// public class TextPopupController : MonoBehaviour
// {
//     public float floatSpeed = 50f;
//     public float fadeDuration = 1f;
    
//     private TextMeshProUGUI tmpText;
//     private RectTransform rectTransform;
//     private Color originalColor;

//     void Awake()
//     {
//         tmpText = GetComponent<TextMeshProUGUI>();
//         rectTransform = GetComponent<RectTransform>();
//         originalColor = tmpText.color;
//     }

//     public void ShowPopup(string message, Color color)
//     {
//         tmpText.text = message;
//         //Set the text colour (max alpha)
//         tmpText.color = new Color(color.r, color.g, color.b, 1f);
//         StartCoroutine(AnimatePopup());
//     }

//     System.Collections.IEnumerator AnimatePopup()
//     {
//         float elapsed = 0f;
//         Vector2 startPos = rectTransform.anchoredPosition;
        
//         while (elapsed < fadeDuration)
//         {
//             elapsed += Time.deltaTime;
//             float t = elapsed / fadeDuration;
            
//             // Move upward over time
//             rectTransform.anchoredPosition = startPos + Vector2.up * floatSpeed * elapsed;
//             // Fade out: alpha goes from 1 to 0
//             float alpha = Mathf.Lerp(1f, 0f, t);
//             tmpText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            
//             yield return null;
//         }
        
//         Destroy(gameObject);
//     }
// }

using UnityEngine;

public class TextPopupController : MonoBehaviour 
{
    [SerializeField] private Transform attackRatePopup;

    private void Start()
    {
        Instantiate(attackRatePopup, Vector3.zero, Quaternion.identity);
    }
}
