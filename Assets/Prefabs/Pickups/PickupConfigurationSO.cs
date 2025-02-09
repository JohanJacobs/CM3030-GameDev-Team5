using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PickupConfigurationSO : ScriptableObject
{
    [Serializable]
    public class PickupConfigEntry
    {
        public GameObject prefab;
        [Range(0, 1)] public float probability;
    }

    public PickupConfigEntry[] PickupConfigs;
}
