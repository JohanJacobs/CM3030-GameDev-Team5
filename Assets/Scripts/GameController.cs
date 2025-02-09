using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UIElements;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public class GameController : MonoBehaviour
{
    private class MonsterSpawnerContext
    {
        private readonly WeakReference<GameController> _gameController;
        private readonly MonsterSpawner _spawner;

        private readonly List<GameObject> _monsters = new List<GameObject>();

        private float _toNextSpawn = 0;

        private GameController GameController => _gameController.TryGetTarget(out var instance) ? instance : null;

        public MonsterSpawnerContext(GameController controller, MonsterSpawner spawner)
        {
            _gameController = new WeakReference<GameController>(controller);
            _spawner = spawner;
        }

        public void Update()
        {
            _toNextSpawn -= Time.deltaTime;

            if (_toNextSpawn > 0)
                return;

            _toNextSpawn += _spawner.SpawnInterval;

            Spawn();
        }
        private void Spawn()
        {
            var maxSpawnAmount = Mathf.Max(_spawner.MaxMonstersAlive - _monsters.Count, 0);
            var amount = Mathf.Clamp(Random.Range(_spawner.SpawnAmountMin, _spawner.SpawnAmountMax + 1), 0, maxSpawnAmount);

            for (var i = 0; i < amount; ++i)
            {
                var monster = GameController.SpawnMonster(_spawner);

                OnMonsterSpawn(monster);
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

    public float SpawnDistance = 30f;

    public MonsterSpawner[] Spawns;

    private GameObject _player;

    private readonly Dictionary<MonsterSpawner, MonsterSpawnerContext> _spawnerContexts = new Dictionary<MonsterSpawner, MonsterSpawnerContext>();

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
        var directionAngle = Random.Range(-Mathf.PI, Mathf.PI);
        var direction = new Vector3(Mathf.Cos(directionAngle), 0, Mathf.Sin(directionAngle));

        var position = _player.transform.position + direction * SpawnDistance;
        var rotation = Quaternion.AngleAxis(-directionAngle, Vector3.up);

        var monsterGameObject = GameObject.Instantiate(spawner.Prefab, position, rotation);
        
        var monster = monsterGameObject.GetComponent<Monster>();

        if (monster == null)
            throw new Exception("Monster prefab must have Monster component");

        return monster;
    }

    private GameObject SpawnExperienceOrbPickup(float experience, Vector3 position)
    {
        var instance = Instantiate(GameData.Instance.ExperienceOrbPickupPrefab, position, Quaternion.identity);

        var pickup = instance.GetComponent<ExperienceOrbPickup>();

        pickup.Experience = experience;

        return instance;
    }

    #region Pickup_spawn
    [Header("Pickup spawn")]
    [SerializeField] private float spawnRadius;
    [SerializeField] private PickupConfigurationSO pickupConfiguration;

    private void SpawnPickups(Vector3 position)
    {
        foreach (var pickup in pickupConfiguration.PickupConfigs)
        {
            if (Random.Range(0,1f) <= pickup.probability)
            {
                SpawnPickup(pickup.prefab, position, spawnRadius);
            }
        }
    }

    private void SpawnPickup(GameObject prefab, Vector3 position, float spawnRadius)
    {
        var position_offset = new Vector3(
                Random.Range(0,spawnRadius),
                0f,
                Random.Range(0, spawnRadius));

        var go = Instantiate(prefab, position + position_offset,Quaternion.identity);
        go.transform.parent = transform;
    }

    #endregion Pickup_spawn
}

