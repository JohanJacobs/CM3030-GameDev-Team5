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
                var prefab = Resources.Load<GameData>("DefaultGameData");

                _instance = Instantiate(prefab);

                DontDestroyOnLoad(_instance);
            }

            return _instance;
        }
    }

    public GameObject ExperienceOrbPickupPrefab;
    public PickupSpawnConfiguration PickupSpawnConfiguration;
    public Player PlayerPrefab;

    private static GameData _instance;
}
