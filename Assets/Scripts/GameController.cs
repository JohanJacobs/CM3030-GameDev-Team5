using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{

    public AudioManager audioManager;

    private class MonsterSpawnerContext
    {

        private readonly WeakReference<GameController> _gameController;
        private readonly MonsterSpawner _spawner;

        private readonly List<GameObject> _monsters = new List<GameObject>();

        private float _toNextSpawn;
        private float _toNextDespawn;

        private GameController GameController => _gameController.TryGetTarget(out var instance) ? instance : null;

        public MonsterSpawnerContext(GameController controller, MonsterSpawner spawner)
        {
            _gameController = new WeakReference<GameController>(controller);
            _spawner = spawner;
        }

        public void Update()
        {
            Spawn();
            Despawn();
        }

        private void Spawn()
        {
            _toNextSpawn -= Time.deltaTime;

            if (_toNextSpawn > 0)
                return;

            _toNextSpawn += _spawner.SpawnInterval;

            var maxSpawnAmount = Mathf.Max(_spawner.MaxMonstersAlive - _monsters.Count, 0);
            var amount = Mathf.Clamp(Random.Range(_spawner.SpawnAmountMin, _spawner.SpawnAmountMax + 1), 0, maxSpawnAmount);

            for (var i = 0; i < amount; ++i)
            {
                var monster = GameController.SpawnMonster(_spawner);

                OnMonsterSpawn(monster);
            }
        }

        // TODO: hacky, should be rewritten
        private void Despawn()
        {
            _toNextDespawn -= Time.deltaTime;

            if (_toNextDespawn > 0)
                return;

            _toNextDespawn += 1f;

            var playerPosition = GameController._player.transform.position;
            var sqrDespawnDistance = GameController.MonsterDespawnDistance * GameController.MonsterDespawnDistance;

            for (var i = 0; i < _monsters.Count;)
            {
                var monsterGameObject = _monsters[i];

                if (Vector3.SqrMagnitude(monsterGameObject.transform.position - playerPosition) > sqrDespawnDistance)
                {
                    _monsters.RemoveAt(i);

                    Destroy(monsterGameObject);
                }
                else
                {
                    ++i;
                }
            }
        }

        private void OnMonsterSpawn(Creature monster)
        {
            monster.Death += OnMonsterDeath;

            var monsterGameObject = monster.gameObject;

            _monsters.Add(monsterGameObject);
        }

        private void OnMonsterDeath(Creature monster)
        {
            monster.Death -= OnMonsterDeath;

            var monsterGameObject = monster.gameObject;

            _monsters.Remove(monsterGameObject);

            DropMonsterLoot(monster);
        }

        private void DropMonsterLoot(Creature monster)
        {
            var experience = GetMonsterExperienceReward(monster);
            if (experience > 0)
            {
                GameController.SpawnExperienceOrbPickup(experience, monster.transform.position);
            }

            GameController.SpawnPickups(monster.transform.position);
        }

        private float GetMonsterExperienceReward(Creature monster)
        {
            var asc = monster.GetComponent<AbilitySystemComponent>();
            if (asc == null)
                return 0f;

            return asc.GetAttributeValue(AttributeType.Experience);
        }
    }

    public float MonsterSpawnDistance = 20f;
    public float MonsterDespawnDistance = 30f;

    public MonsterSpawner[] Spawns;

    private GameObject _player;

    private readonly Dictionary<MonsterSpawner, MonsterSpawnerContext> _spawnerContexts = new Dictionary<MonsterSpawner, MonsterSpawnerContext>();

    void Awake()
    {
        _navMeshWalkableAreaMask = (1 << NavMesh.GetAreaFromName("Walkable"));

        // Set audioManager to external audioManager object with tag Audio
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (_player == null)
            return;

        foreach (var spawner in Spawns)
        {
            UpdateMonsterSpawner(spawner);
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

    private void UpdateMonsterSpawner(MonsterSpawner spawner)
    {
        if (_spawnerContexts.TryGetValue(spawner, out var context))
        {
        }
        else
        {
            context = new MonsterSpawnerContext(this, spawner);

            _spawnerContexts.Add(spawner, context);
        }

        context.Update();
    }

    private Monster SpawnMonster(MonsterSpawner spawner)
    {
        var position = GetMonsterSpawnPoint(out var directionAngle);
        var rotation = Quaternion.AngleAxis(-directionAngle, Vector3.up);

        var instance = Instantiate(spawner.Prefab, position, rotation);

        instance.transform.parent = transform;

        var monster = instance.GetComponent<Monster>();

        Debug.Assert(monster != null, "Monster component is missing");

        return monster;
    }

    #endregion

    #region Pickups Spawn
    [Header("Pickup spawn")]

    [SerializeField]
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

        instance.transform.parent = transform;
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
}

