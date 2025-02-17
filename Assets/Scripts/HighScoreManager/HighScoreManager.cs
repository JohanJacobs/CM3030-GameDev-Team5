using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class HighScoreManager :MonoBehaviour
{
    private static HighScoreManager _instance;
    public static HighScoreManager Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = new GameObject().AddComponent<HighScoreManager>();
                _instance.name = "HighScoreManager";
                DontDestroyOnLoad(_instance.gameObject);
            }
            return _instance;
        }
    }

    private int lastHighScore=0;
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
