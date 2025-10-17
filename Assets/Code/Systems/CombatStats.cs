#nullable enable
using System;
using UnityEngine;

namespace VHDPV2.Systems
{
    [Serializable]
    public sealed class CombatStats
    {
        [SerializeField] private float health = 10f;
        [SerializeField] private float armor;
        [SerializeField] private float critChance;
        [SerializeField] private float critMultiplier = 2f;

        public float Health
        {
            get => health;
            set => health = Mathf.Max(0f, value);
        }

        public float Armor
        {
            get => armor;
            set => armor = Mathf.Max(0f, value);
        }

        public float CritChance
        {
            get => critChance;
            set => critChance = Mathf.Clamp01(value);
        }

        public float CritMultiplier
        {
            get => critMultiplier;
            set => critMultiplier = Mathf.Max(1f, value);
        }
    }
}
