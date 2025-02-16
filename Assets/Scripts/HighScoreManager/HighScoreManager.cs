using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class HighScoreManager :MonoBehaviour
{
    public static HighScoreManager Instance { get; private set; }

    private int lastHighScore=0;

    private void Awake()
    {

        if (Instance != this && Instance != null)
        { 
            // somehow this is a second instance ?
            Destroy(this);
            return;
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public void AddNewScore(int score)
    {
        if (score > lastHighScore)
        {
            lastHighScore = score;            
        }
    }

    public int GetHighestScore()
    {
        return lastHighScore;
    }
}
