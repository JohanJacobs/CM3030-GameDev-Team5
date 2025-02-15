using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using TMPro;
using UnityEngine.UI;
using UnityEditor;
public class GameMenu : MonoBehaviour
{
    [SerializeField] GameObject Menu;
    [SerializeField] GameObject MiniUIPanel;    
    [SerializeField] Text startButtonText;
    
    bool menuVisibleState;
    bool switchStartButton=false;
    System.Action<bool> toggleHudVisibile;

    public void Start()
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
        // menu 
        menuVisibleState = menuState;
        Menu.SetActive(menuState);
        MiniUIPanel.SetActive(!menuState);

        // game state 
        Time.timeScale = (!menuState)?1f:0f;

        // update the hud 
        toggleHudVisibile.Invoke(!menuState);

    }

    public void QuitGame()
    {
        if (Application.isEditor)
        {
            EditorApplication.isPlaying = false;
        }
        else
        {
            Application.Quit();
        }
    }

    public void SetHudVisibilityToggleCallback(System.Action<bool> ToggleHudCallbackAction)
    {
        toggleHudVisibile = ToggleHudCallbackAction;
    }
}
