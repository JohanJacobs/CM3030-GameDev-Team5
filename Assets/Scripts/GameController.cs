/*

University of London
BsC Computer Science Course
Games Development
Final Assignment - Streets of Fire Game

Group 5 

Please refer to the README file for detailled information

GameController.cs

Class GameController deals with minion and loot spawning and some audio effects

*/

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour, IMonsterSpawnHandler
{
    public static GameController ActiveInstance => _activeInstance;

    public Tag PlayerStartTag = "PlayerStart";

    public float MonsterSpawnDistance = 20f;
    public float MonsterDespawnDistance = 30f;
    public MonsterSpawner[] Spawns;

    public GameObject Player => _player;

    private static GameController _activeInstance;

    private readonly List<MonsterSpawnerInstance> _monsterSpawnerInstances = new List<MonsterSpawnerInstance>();

    private GameObject _player;

    private GameTimer _gameTimer;

    [SerializeField] float gameTimeInMinutes = 1f;

    void Awake()
    {
        // set static instance
        {
            Debug.Assert(_activeInstance == null);

            _activeInstance = this;
        }

        _navMeshWalkableAreaMask = (1 << NavMesh.GetAreaFromName("Walkable"));
    }

    void Start()
    {
        AudioManager.Instance.Initialize();

        SpawnPlayer();

        var monsterSpawnerInstances = Spawns
            .Select(monsterSpawner => new MonsterSpawnerInstance(monsterSpawner, this));

        _monsterSpawnerInstances.AddRange(monsterSpawnerInstances);

        SetupGameTimer();
    }

    void Update()
    {
        if (_player == null)
            return;

        _gameTimer.Update();

        foreach (var monsterSpawnerInstance in _monsterSpawnerInstances)
        {
            monsterSpawnerInstance.Update();
        }
    }

    void OnDestroy()
    {
        // reset static instance
        {
            Debug.Assert(_activeInstance == this);

            _activeInstance = null;
        }
    }

    #region Monster Spawn

    private static int _navMeshWalkableAreaMask;

    private static Vector3 GetRandomPointOnUnitCircle(out float angle)
    {
        angle = Random.Range(-Mathf.PI, Mathf.PI);

        return new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
    }

    private Vector3 GetMonsterSpawnPoint(out float angle)
    {
        var playerPositionXZ = _player.transform.position;

        playerPositionXZ.y = 0;

        Vector3 point;

        int attempt = 0;

        do
        {
            point = playerPositionXZ + GetRandomPointOnUnitCircle(out angle) * MonsterSpawnDistance;

            if (NavMesh.SamplePosition(point, out var hit, 1f, _navMeshWalkableAreaMask))
            {
                return hit.position;
            }
        } while (++attempt < 5);

        Debug.LogAssertion("Failed to generate monster spawn point");

        return point;
    }

    #endregion

    #region Pickups Spawn

    [Header("Pickup spawn")] [SerializeField]
    private float _pickupSpawnRadius = 2f;

    private void SpawnPickups(Vector3 position)
    {
        foreach (var configEntry in GameData.Instance.PickupSpawnConfiguration.PickupConfigs)
        {
            if (Random.Range(0, 1f) > configEntry.probability)
                continue;

            SpawnPickup(configEntry.prefab, position, _pickupSpawnRadius);
        }
    }

    private void SpawnPickup(GameObject prefab, Vector3 position, float spawnRadius)
    {
        var positionOffset = new Vector3(Random.Range(-spawnRadius, spawnRadius), 0f, Random.Range(-spawnRadius, spawnRadius));

        var instance = Instantiate(prefab, position + positionOffset, Quaternion.identity);
    }

    private GameObject SpawnExperienceOrbPickup(float experience, Vector3 position)
    {
        var instance = Instantiate(GameData.Instance.ExperienceOrbPickupPrefab, position, Quaternion.identity);

        instance.transform.parent = transform;

        var pickup = instance.GetComponent<ExperienceOrbPickup>();

        Debug.Assert(pickup != null, "Pickup component is missing");

        pickup.Experience = experience;

        return instance;
    }

    #endregion Pickups Spawn

    #region IMonsterSpawnHandler

    public Creature SpawnMonster(MonsterSpawnerInstance spawnerInstance, GameObject prefab)
    {
        var position = GetMonsterSpawnPoint(out var directionAngle);
        var rotation = Quaternion.AngleAxis(-directionAngle, Vector3.up);

        var instance = Instantiate(spawnerInstance.Spawner.Prefab, position, rotation);

        instance.transform.parent = transform;

        var monster = instance.GetComponent<Monster>();

        Debug.Assert(monster != null, "Monster component is missing");

        return monster;
    }

    public bool ShouldDespawnMonster(MonsterSpawnerInstance spawnerInstance, Creature monster)
    {
        var position = monster.transform.position;

        return Vector3.SqrMagnitude(position - _player.transform.position) > MonsterDespawnDistance * MonsterDespawnDistance;
    }

    public void DropMonsterLoot(MonsterSpawnerInstance spawnerInstance, Creature monster)
    {
        var position = monster.transform.position;

        var experience = monster.AbilitySystemComponent.GetAttributeValue(AttributeType.Experience);
        if (experience > 0)
        {
            SpawnExperienceOrbPickup(experience, position);
        }

        SpawnPickups(position);
    }

    #endregion

    private Transform PickPlayerStart()
    {
        var playerStarts = GameObject.FindGameObjectsWithTag((string)PlayerStartTag);

        if (playerStarts == null || playerStarts.Length == 0)
            return null;

        var playerStartIndex = Random.Range(0, playerStarts.Length);

        return playerStarts[playerStartIndex].transform;
    }

    private void SpawnPlayer()
    {
        Vector3 position;
        Quaternion rotation;

        var playerStart = PickPlayerStart();
        if (playerStart == null)
        {
            Debug.LogWarning("No player starts");

            position = Vector3.zero;
            rotation = Quaternion.identity;
        }
        else
        {
            position = playerStart.position;
            rotation = playerStart.rotation;
        }

        var player = Instantiate(GameData.Instance.PlayerPrefab, position, rotation);

        _player = player.gameObject;
    }

    #region GameTimerRelated
    private void SetupGameTimer()
    {
        // Setup the game timer to control the rounds in the game                 
        _gameTimer = new GameTimer(gameTimeInMinutes * 60);
    }
    #endregion GameTimerRelated
}