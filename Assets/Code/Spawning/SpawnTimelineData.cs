#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;
using VHDPV2.Enemies;

namespace VHDPV2.Spawning
{
    [CreateAssetMenu(fileName = "SpawnTimeline", menuName = "VHD/Spawning/Timeline")]
    public sealed class SpawnTimelineData : ScriptableObject
    {
        [SerializeField] private List<SpawnSlice> slices = new();

        public IReadOnlyList<SpawnSlice> Slices => slices;
    }

    [Serializable]
    public sealed class SpawnSlice
    {
        public float TimeStart;
        public float TimeEnd;
        public List<SpawnEntry> Entries = new();
        public float SpawnInterval = 1f;
    }

    [Serializable]
    public sealed class SpawnEntry
    {
        public EnemyData? Enemy;
        public int Weight = 1;
    }
}
