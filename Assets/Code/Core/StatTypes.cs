#nullable enable
using System;
using System.Collections.Generic;

namespace VHDPV2.Core
{
    public enum StatType
    {
        MaxHealth,
        Armor,
        MoveSpeed,
        PickupRange,
        CritChance,
        CritMultiplier,
        Magnet,
        CooldownMultiplier,
        ProjectileSpeed,
        Area,
        Luck,
        ExperienceGainMultiplier,
        DamageMultiplier,
        AttackSpeedMultiplier
    }

    public enum StatOperation
    {
        Add,
        Multiply
    }

    public sealed class StatCollection
    {
        private readonly Dictionary<StatType, float> _additive = new();
        private readonly Dictionary<StatType, float> _multiplicative = new();

        public StatCollection()
        {
            foreach (StatType stat in Enum.GetValues(typeof(StatType)))
            {
                _additive[stat] = 0f;
                _multiplicative[stat] = 1f;
            }
        }

        public StatCollection Clone()
        {
            var clone = new StatCollection();
            foreach (var kvp in _additive)
            {
                clone._additive[kvp.Key] = kvp.Value;
            }

            foreach (var kvp in _multiplicative)
            {
                clone._multiplicative[kvp.Key] = kvp.Value;
            }

            return clone;
        }

        public void ApplyModifier(StatType type, StatOperation operation, float value)
        {
            switch (operation)
            {
                case StatOperation.Add:
                    _additive[type] += value;
                    break;
                case StatOperation.Multiply:
                    _multiplicative[type] *= value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(operation), operation, null);
            }
        }

        public float GetValue(StatType type, float baseValue)
        {
            return (baseValue + _additive[type]) * _multiplicative[type];
        }

        public void Reset()
        {
            foreach (StatType stat in Enum.GetValues(typeof(StatType)))
            {
                _additive[stat] = 0f;
                _multiplicative[stat] = 1f;
            }
        }
    }
}
