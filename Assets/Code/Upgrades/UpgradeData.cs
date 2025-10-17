#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;
using VHDPV2.Core;
using VHDPV2.Effects;

namespace VHDPV2.Upgrades
{
    public enum UpgradeCategory
    {
        Weapon,
        Passive,
        Evolution
    }

    [CreateAssetMenu(fileName = "UpgradeData", menuName = "VHD/Upgrades/Upgrade")]
    public sealed class UpgradeData : ScriptableObject
    {
        [SerializeField] private string id = string.Empty;
        [SerializeField] private string displayName = string.Empty;
        [SerializeField] private UpgradeCategory category;
        [SerializeField] private List<StatModifier> statModifiers = new();
        [SerializeField] private int maxRank = 5;
        [SerializeField] private Sprite? icon;

        public string Id => id;
        public string DisplayName => displayName;
        public UpgradeCategory Category => category;
        public IReadOnlyList<StatModifier> StatModifiers => statModifiers;
        public int MaxRank => maxRank;
        public Sprite? Icon => icon;
    }

    [Serializable]
    public sealed class StatModifier
    {
        public StatType Stat;
        public StatOperation Operation = StatOperation.Add;
        public float Value = 1f;
    }
}
