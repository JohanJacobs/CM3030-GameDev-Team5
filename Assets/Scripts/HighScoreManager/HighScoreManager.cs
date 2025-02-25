using UnityEngine;


/* 
    Singleton pattern based on implementation from kurtdekker/SingletonSimple.cs
    https://gist.github.com/kurtdekker/775bb97614047072f7004d6fb9ccce30
 */

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
