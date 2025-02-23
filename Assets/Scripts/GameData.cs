/*

University of London
BsC Computer Science Course
Games Development
Final Assignment - Streets of Fire Game

Group 5 

Please refer to the README file for detailled information

GameData.cs

class GameData loads configuration

*/

using UnityEngine;

[CreateAssetMenu]
public class GameData : ScriptableObject
{
    public static GameData Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<GameData>("DefaultGameData");
            }

            return _instance;
        }
    }

    public GameObject ExperienceOrbPickupPrefab;
    public PickupSpawnConfiguration PickupSpawnConfiguration;

    private static GameData _instance;
}
