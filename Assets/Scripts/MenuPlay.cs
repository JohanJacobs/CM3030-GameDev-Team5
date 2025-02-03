using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPlay: MonoBehaviour
{
    public void PlayGame() 
    {
        SceneManager.LoadSceneAsync("GameScene");
    }

    public void SetSounds()
    {
        SceneManager.LoadSceneAsync("SettingsMenu");
    }

    public void QuitGame() 
    {
        Application.Quit();
    }

    public void BackMain()
    {
        SceneManager.LoadSceneAsync("MainMenu");
    }    
}
