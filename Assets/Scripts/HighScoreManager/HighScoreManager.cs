using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighScoreManager :MonoBehaviour
{
    int lastHighScore;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void AddNewScore(int score)
    {
       if (score > lastHighScore)
           lastHighScore = score; 
    }

    public int GetHighestScore()
    {
        return lastHighScore;
    }
}
