using UnityEngine;

[CreateAssetMenu]
public class MonsterSpawner : ScriptableObject
{
    /// <summary>
    /// Monster prefab
    /// </summary>
    public GameObject Prefab;
    /// <summary>
    /// Interval (seconds) between spawn batches
    /// </summary>
    public float SpawnInterval = 3;
    /// <summary>
    /// Spawn batch minimum size
    /// </summary>
    public int SpawnAmountMin = 2;
    /// <summary>
    /// Spawn batch maximum size
    /// </summary>
    public int SpawnAmountMax = 5;
    /// <summary>
    /// Population limit of this spawner
    /// </summary>
    public int MaxMonstersAlive = 15;
    /// <summary>
    /// Delay (seconds) before this spawner starts to spawn
    /// </summary>
    public float SpawnDelay = 0;
    /// <summary>
    /// Spawner lifespan (seconds) since start. 0 means no limit.
    /// </summary>
    public float SpawnTime = 0;

    public bool HasLifespan => SpawnTime > 0;

#if UNITY_EDITOR
    private void OnValidate()
    {
        SpawnInterval = Mathf.Max(SpawnInterval, 0);
        SpawnAmountMin = Mathf.Max(SpawnAmountMin, 0);
        SpawnAmountMax = Mathf.Max(SpawnAmountMax, 1);
        MaxMonstersAlive = Mathf.Max(MaxMonstersAlive, 0);
        SpawnDelay = Mathf.Max(SpawnDelay, 0);
        SpawnTime = Mathf.Max(SpawnTime, 0);

        if (SpawnAmountMax < SpawnAmountMin)
            SpawnAmountMax = SpawnAmountMin;
    }
#endif
}
