using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* 
 * simple class to toggle between two sprites when a toggle method is called
 * the main use case is for the Mute button to switch between the mute and not mute 
 */
public class ToggleBetweenSprite : MonoBehaviour
{
    [SerializeField] Sprite DefaultSprite;
    [SerializeField] Sprite TogggledSprite;
    Image īmage;
    bool showDefaultSprite;

    private void Start()
    {
        showDefaultSprite = true;
        īmage = GetComponent<Image>();
    }


    public void ToggleSprite()
    {        
        showDefaultSprite = !showDefaultSprite;
        īmage.sprite = showDefaultSprite? DefaultSprite : TogggledSprite;
    }
}
