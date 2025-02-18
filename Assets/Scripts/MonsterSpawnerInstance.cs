using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public interface IMonsterSpawnHandler
{
    Creature SpawnMonster(MonsterSpawnerInstance spawnerInstance, GameObject prefab);
    bool ShouldDespawnMonster(MonsterSpawnerInstance spawnerInstance, Creature monster);
    void DropMonsterLoot(MonsterSpawnerInstance spawnerInstance, Creature monster);
}

public class MonsterSpawnerInstance
{
    public MonsterSpawner Spawner;

    private const float DespawnInterval = 1;

    private readonly WeakReference<IMonsterSpawnHandler> _weakHandler;

    private readonly List<Creature> _monsters = new List<Creature>();

    private float _timeToNextSpawn;
    private float _timeToNextDespawn;
    private float _timeToStopSpawning;

    private bool _started;
    private bool _stopped;

    private bool HasAliveMonsters => _monsters.Count > 0;

    public MonsterSpawnerInstance(MonsterSpawner spawner, IMonsterSpawnHandler handler)
    {
        Spawner = spawner;

        _weakHandler = new WeakReference<IMonsterSpawnHandler>(handler);

        _timeToNextSpawn = Mathf.Max(Spawner.SpawnDelay, 0f);
        _timeToNextDespawn = Mathf.Max(DespawnInterval, 0f);
        _timeToStopSpawning = Mathf.Max(Spawner.SpawnTime, 0f);

        _started = !(_timeToNextSpawn > 0);
    }

    public void Update()
    {
        var deltaTime = Time.deltaTime;

        if (_stopped)
        {
            if (HasAliveMonsters)
            {
                TickDespawn(deltaTime);
            }

            return;
        }

        if (_started)
        {
            TickSpawn(deltaTime);
            TickDespawn(deltaTime);

            if (Spawner.HasLifespan)
            {
                TickLifespan(deltaTime);
            }
        }
        else
        {
            _timeToNextSpawn -= deltaTime;

            if (_timeToNextSpawn > 0)
                return;

            _started = true;

            TickSpawn(0);
        }
    }

    private void TickSpawn(float deltaTime)
    {
        _timeToNextSpawn -= deltaTime;

        if (_timeToNextSpawn > 0)
            return;

        _timeToNextSpawn += Spawner.SpawnInterval;

        Spawn();
    }

    private void TickDespawn(float deltaTime)
    {
        _timeToNextDespawn -= deltaTime;

        if (_timeToNextDespawn > 0)
            return;

        _timeToNextDespawn += DespawnInterval;

        Despawn();
    }

    private void TickLifespan(float deltaTime)
    {
        _timeToStopSpawning -= deltaTime;

        if (_timeToStopSpawning > 0)
            return;

        _stopped = true;
    }

    private void Spawn()
    {
        if (!_weakHandler.TryGetTarget(out var handler))
            return;

        var amount = Random.Range(Spawner.SpawnAmountMin, Spawner.SpawnAmountMax + 1);

        if (Spawner.MaxMonstersAlive > 0)
        {
            var slots = Spawner.MaxMonstersAlive - _monsters.Count;

            if (amount > slots)
                amount = slots;
        }

        for (int i = 0; i < amount; ++i)
        {
            var monster = handler.SpawnMonster(this, Spawner.Prefab);
            if (monster)
            {
                OnMonsterSpawn(monster);
            }
        }
    }

    private void Despawn()
    {
        if (!_weakHandler.TryGetTarget(out var handler))
            return;

        _monsters
            .Where(monster => handler.ShouldDespawnMonster(this, monster))
            .ToList()
            .ForEach(OnMonsterDespawn);
    }

    private void OnMonsterSpawn(Creature creature)
    {
        creature.Death += OnMonsterDeath;

        _monsters.Add(creature);
    }

    private void OnMonsterDespawn(Creature creature)
    {
        _monsters.Remove(creature);

        creature.Death -= OnMonsterDeath;

        Object.Destroy(creature.gameObject);
    }

    private void OnMonsterDeath(Creature creature)
    {
        _monsters.Remove(creature);

        creature.Death -= OnMonsterDeath;

        if (!_weakHandler.TryGetTarget(out var handler))
            return;

        handler.DropMonsterLoot(this, creature);
    }
}