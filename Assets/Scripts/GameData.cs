using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameInputData
{
    public Tag MainHandAbilityInputTag;
    public Tag OffHandAbilityInputTag;
}

[Serializable]
public class GameAttributeData
{
    public AttributeType Attribute;
    public string DisplayName;
    public string Description;
}

[Serializable]
public class GameLayersData
{
    public LayerMask TerrainMask;
    public LayerMask EnvironmentMask;
    public LayerMask PlayerMask;
    public LayerMask MonsterMask;
    public LayerMask PickupMask;
    public LayerMask ProjectileMask;
}

[Serializable]
public class GameTags
{
    public Tag ConditionDead;
    public Tag ConditionStunned;
}

[CreateAssetMenu]
public class GameData : ScriptableObject, ISerializationCallbackReceiver
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
    public GameObject hitParticleEffect;
    public Player PlayerPrefab;
    public GameInputData InputData;
    public GameAttributeData[] AttributeData;
    public GameLayersData LayersData;
    public GameTags Tags;

    public IReadOnlyDictionary<AttributeType, GameAttributeData> AttributeDataMap => _attributeDataMap;

    public void OnBeforeSerialize()
    {
    }

    public void OnAfterDeserialize()
    {
        _attributeDataMap.Clear();

        if (AttributeData != null)
        {
            foreach (var attributeData in AttributeData)
            {
                _attributeDataMap[attributeData.Attribute] = attributeData;
            }
        }
    }

    private static GameData _instance;

    private readonly Dictionary<AttributeType, GameAttributeData> _attributeDataMap = new Dictionary<AttributeType, GameAttributeData>();
}
