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
    public GameObject hitParticleEffect;

    private static GameData _instance;
}
