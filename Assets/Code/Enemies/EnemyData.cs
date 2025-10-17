#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;
using VHDPV2.Effects;
using VHDPV2.Pickups;

namespace VHDPV2.Enemies
{
    public enum EnemyBehaviorType
    {
        Seek,
        Charge,
        Kite
    }

    [CreateAssetMenu(fileName = "EnemyData", menuName = "VHD/Enemy Data")]
    public sealed class EnemyData : ScriptableObject
    {
        [SerializeField] private string id = string.Empty;
        [SerializeField] private float health = 10f;
        [SerializeField] private float damage = 5f;
        [SerializeField] private float speed = 2f;
        [SerializeField] private float armor;
        [SerializeField] private EnemyBehaviorType behaviorType;
        [SerializeField] private GameObject? prefab;
        [SerializeField] private PickupDropTable lootTable = new();
        [SerializeField] private StatusEffectData? onHitEffect;

        public string Id => id;
        public float Health => health;
        public float Damage => damage;
        public float Speed => speed;
        public float Armor => armor;
        public EnemyBehaviorType BehaviorType => behaviorType;
        public GameObject? Prefab => prefab;
        public PickupDropTable LootTable => lootTable;
        public StatusEffectData? OnHitEffect => onHitEffect;
    }

    [Serializable]
    public sealed class PickupDropTableEntry
    {
        public PickupItem? Item;
        public int Weight = 1;
    }

    [Serializable]
    public sealed class PickupDropTable
    {
        public List<PickupDropTableEntry> Entries = new();

        public PickupItem? Roll()
        {
            int totalWeight = 0;
            foreach (PickupDropTableEntry entry in Entries)
            {
                totalWeight += entry.Weight;
            }

            if (totalWeight <= 0)
            {
                return null;
            }

            int roll = UnityEngine.Random.Range(0, totalWeight);
            int cumulative = 0;
            foreach (PickupDropTableEntry entry in Entries)
            {
                cumulative += entry.Weight;
                if (roll < cumulative)
                {
                    return entry.Item;
                }
            }

            return null;
        }
    }
}
