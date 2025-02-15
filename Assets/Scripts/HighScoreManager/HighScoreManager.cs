using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighScoreManager :MonoBehaviour
{
    private static int lastHighScore=0;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);        
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
