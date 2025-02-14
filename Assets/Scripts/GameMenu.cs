using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using TMPro;
using UnityEngine.UI;
public class GameMenu : MonoBehaviour
{
    [SerializeField] GameObject Menu;
    [SerializeField] GameObject MiniUIPanel;    
    [SerializeField] Text startButtonText;
    
    bool menuVisibleState;
    bool switchStartButton=false;

    public void Awake()
    {
        
        SetMenuState(true);
        switchStartButton= true;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMenu();
        }
    }
    public void ToggleMenu()
    {
        SetMenuState(!menuVisibleState);

        if (switchStartButton)
        {
            startButtonText.text = "Continue";
            switchStartButton = false;
        }
    }

    private void SetMenuState(bool menuState)
    { 
        menuVisibleState = menuState;
        Menu.SetActive(menuState);
        MiniUIPanel.SetActive(!menuState);
        Time.timeScale = (!menuState)?1f:0f;
    }
}
