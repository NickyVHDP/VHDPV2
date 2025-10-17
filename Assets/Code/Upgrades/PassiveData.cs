#nullable enable
using System.Collections.Generic;
using UnityEngine;
using VHDPV2.Core;

namespace VHDPV2.Upgrades
{
    [CreateAssetMenu(fileName = "PassiveData", menuName = "VHD/Upgrades/Passive")]
    public sealed class PassiveData : ScriptableObject
    {
        [SerializeField] private string id = string.Empty;
        [SerializeField] private List<StatModifier> modifiers = new();
        [SerializeField] private int maxRank = 5;

        public string Id => id;
        public IReadOnlyList<StatModifier> Modifiers => modifiers;
        public int MaxRank => maxRank;
    }
}
