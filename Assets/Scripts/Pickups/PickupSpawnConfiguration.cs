using System;
using UnityEngine;

[CreateAssetMenu]
public class PickupSpawnConfiguration : ScriptableObject
{
    [Serializable]
    public class PickupConfigEntry
    {
        public GameObject prefab;

        [Range(0, 1)]
        public float probability;
    }

    public PickupConfigEntry[] PickupConfigs;
}
