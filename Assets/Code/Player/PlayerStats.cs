#nullable enable
using System;
using UnityEngine;
using VHDPV2.Core;

namespace VHDPV2.Player
{
    [Serializable]
    public sealed class PlayerStats
    {
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float armor;
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float pickupRange = 2f;
        [SerializeField] private float critChance = 0.05f;
        [SerializeField] private float critMultiplier = 2f;
        [SerializeField] private float magnet = 1f;
        [SerializeField] private float cooldownMultiplier = 1f;
        [SerializeField] private float projectileSpeed = 10f;
        [SerializeField] private float area = 1f;
        [SerializeField] private float luck = 0f;
        [SerializeField] private float experienceGainMultiplier = 1f;
        [SerializeField] private float damageMultiplier = 1f;
        [SerializeField] private float attackSpeedMultiplier = 1f;

        public float MaxHealth => maxHealth;
        public float Armor => armor;
        public float MoveSpeed => moveSpeed;
        public float PickupRange => pickupRange;
        public float CritChance => critChance;
        public float CritMultiplier => critMultiplier;
        public float Magnet => magnet;
        public float CooldownMultiplier => cooldownMultiplier;
        public float ProjectileSpeed => projectileSpeed;
        public float Area => area;
        public float Luck => luck;
        public float ExperienceGainMultiplier => experienceGainMultiplier;
        public float DamageMultiplier => damageMultiplier;
        public float AttackSpeedMultiplier => attackSpeedMultiplier;

        public StatCollection BuildStatCollection()
        {
            var collection = new StatCollection();
            collection.ApplyModifier(StatType.MaxHealth, StatOperation.Add, maxHealth);
            collection.ApplyModifier(StatType.Armor, StatOperation.Add, armor);
            collection.ApplyModifier(StatType.MoveSpeed, StatOperation.Add, moveSpeed);
            collection.ApplyModifier(StatType.PickupRange, StatOperation.Add, pickupRange);
            collection.ApplyModifier(StatType.CritChance, StatOperation.Add, critChance);
            collection.ApplyModifier(StatType.CritMultiplier, StatOperation.Add, critMultiplier);
            collection.ApplyModifier(StatType.Magnet, StatOperation.Add, magnet);
            collection.ApplyModifier(StatType.CooldownMultiplier, StatOperation.Multiply, cooldownMultiplier);
            collection.ApplyModifier(StatType.ProjectileSpeed, StatOperation.Add, projectileSpeed);
            collection.ApplyModifier(StatType.Area, StatOperation.Add, area);
            collection.ApplyModifier(StatType.Luck, StatOperation.Add, luck);
            collection.ApplyModifier(StatType.ExperienceGainMultiplier, StatOperation.Multiply, experienceGainMultiplier);
            collection.ApplyModifier(StatType.DamageMultiplier, StatOperation.Multiply, damageMultiplier);
            collection.ApplyModifier(StatType.AttackSpeedMultiplier, StatOperation.Multiply, attackSpeedMultiplier);
            return collection;
        }
    }
}
