using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable, CreateAssetMenu]
public class MonsterSpawner : ScriptableObject
{
    public GameObject Prefab;
    public float SpawnInterval = 3;
    public int SpawnAmountMin = 2;
    public int SpawnAmountMax = 5;
    public int MaxMonstersAlive = 15;
}
