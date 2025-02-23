/*

University of London
BsC Computer Science Course
Games Development
Final Assignment - Streets of Fire Game

Group 5 

PickupSpawnConfiguration.cs

*/

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
