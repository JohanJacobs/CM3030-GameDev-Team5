/*

University of London
BsC Computer Science Course
Games Development
Final Assignment - Streets of Fire Game

Group 5 

Please refer to the README file for detailled information

GameMenu.cs

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using TMPro;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.SceneManagement;
public class GameMenu : MonoBehaviour
{
    [SerializeField] GameObject Menu;
    [SerializeField] GameObject MiniUIPanel;    
    [SerializeField] Text startButtonText;
    
    
    bool switchStartButton=false;
    System.Action<bool> toggleHudVisibile;
    public bool menuVisibleState = true;

    
    private void OnEnable()
    {
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
    }
    // bit hacky but if you hit restart make sure the menu isn't displayed to avoid making this "dontDestroyOnLoad" 
    // TODO: there should be a UI manager instead
    private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (loadSceneMode == LoadSceneMode.Single) 
        {
            // secene was reloaded disable the menu
            SetMenuState(false);
        }
    }

    public void Start()
    {
        SetMenuState(menuVisibleState);
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

        // Rename the start button to continue after the player press play
        if (switchStartButton)
        {
            startButtonText.text = "Continue";
            switchStartButton = false;
        }

        // Hide the curse when the menu is disabled, else show the cursor when the menu is visible.
        UnityEngine.Cursor.visible = menuVisibleState;
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

        //https://docs.unity3d.com/6000.0/Documentation/Manual/platform-dependent-compilation.html
#if UNITY_EDITOR
        if (Application.isEditor)
        {

            EditorApplication.isPlaying = false;
        }
        else
#endif
        {
            Application.Quit();
        }
    }

    public void SetHudVisibilityToggleCallback(System.Action<bool> ToggleHudCallbackAction)
    {
        toggleHudVisibile = ToggleHudCallbackAction;
    }
}
