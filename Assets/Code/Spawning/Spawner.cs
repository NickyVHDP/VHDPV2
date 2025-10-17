#nullable enable
using System.Collections.Generic;
using UnityEngine;
using VHDPV2.Core;
using VHDPV2.Enemies;

namespace VHDPV2.Spawning
{
    public sealed class Spawner : MonoBehaviour
    {
        [SerializeField] private SpawnTimelineData? timeline;
        [SerializeField] private Transform? player;
        [SerializeField] private float spawnRadius = 10f;
        [SerializeField] private float safeRadius = 4f;
        [SerializeField] private int densityCap = 150;

        private readonly List<GameObject> _activeEnemies = new();
        private float _timer;
        private float _elapsed;

        private void OnEnable()
        {
            _elapsed = 0f;
            _timer = 0f;
        }

        private void Update()
        {
            _elapsed += Time.deltaTime;
            if (timeline == null)
            {
                return;
            }

            CleanActiveList();
            if (_activeEnemies.Count >= densityCap)
            {
                return;
            }

            SpawnSlice? slice = GetCurrentSlice(_elapsed);
            if (slice == null)
            {
                return;
            }

            _timer -= Time.deltaTime;
            if (_timer <= 0f)
            {
                SpawnEnemy(slice);
                _timer = Mathf.Max(0.1f, slice.SpawnInterval);
            }
        }

        private void SpawnEnemy(SpawnSlice slice)
        {
            EnemyData? enemy = RollEnemy(slice.Entries);
            if (enemy == null || enemy.Prefab == null)
            {
                return;
            }

            Vector2 spawnPosition = FindSpawnPosition();
            GameObject instance = Instantiate(enemy.Prefab, spawnPosition, Quaternion.identity);
            _activeEnemies.Add(instance);
        }

        private Vector2 FindSpawnPosition()
        {
            Vector2 basePosition = player != null ? (Vector2)player.position : (Vector2)transform.position;
            for (int attempt = 0; attempt < 10; attempt++)
            {
                Vector2 offset = Random.insideUnitCircle.normalized * spawnRadius;
                Vector2 candidate = basePosition + offset;
                if (player != null && Vector2.Distance(candidate, player.position) < safeRadius)
                {
                    continue;
                }

                return candidate;
            }

            return basePosition + Random.insideUnitCircle * spawnRadius;
        }

        private EnemyData? RollEnemy(IReadOnlyList<SpawnEntry> entries)
        {
            int totalWeight = 0;
            foreach (SpawnEntry entry in entries)
            {
                totalWeight += entry.Weight;
            }

            if (totalWeight <= 0)
            {
                return null;
            }

            int roll = Random.Range(0, totalWeight);
            int cumulative = 0;
            foreach (SpawnEntry entry in entries)
            {
                cumulative += entry.Weight;
                if (roll < cumulative)
                {
                    return entry.Enemy;
                }
            }

            return null;
        }

        private SpawnSlice? GetCurrentSlice(float time)
        {
            foreach (SpawnSlice slice in timeline!.Slices)
            {
                if (time >= slice.TimeStart && time < slice.TimeEnd)
                {
                    return slice;
                }
            }

            return null;
        }

        private void CleanActiveList()
        {
            for (int i = _activeEnemies.Count - 1; i >= 0; i--)
            {
                if (_activeEnemies[i] == null)
                {
                    _activeEnemies.RemoveAt(i);
                }
            }
        }
    }
}
