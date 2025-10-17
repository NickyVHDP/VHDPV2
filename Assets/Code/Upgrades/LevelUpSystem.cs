#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;
using VHDPV2.Core;
using VHDPV2.Player;
using VHDPV2.Systems;
using VHDPV2.Utils;
using VHDPV2.Weapons;

namespace VHDPV2.Upgrades
{
    public sealed class LevelUpSystem : MonoBehaviour
    {
        [SerializeField] private ExperienceSystem? experienceSystem;
        [SerializeField] private WeaponSystem? weaponSystem;
        [SerializeField] private List<UpgradeData> upgradePool = new();
        [SerializeField] private int draftCountMin = 3;
        [SerializeField] private int draftCountMax = 5;
        [SerializeField] private int rerolls = 3;
        [SerializeField] private int skips = 2;

        private readonly Dictionary<string, int> _upgradeRanks = new();
        private readonly List<UpgradeData> _draftBuffer = new();
        private PlayerController? _player;

        public event Action<IReadOnlyList<UpgradeData>>? DraftReady;
        public event Action<int>? RerollsChanged;
        public event Action<int>? SkipsChanged;

        private void Awake()
        {
            _player = GetComponent<PlayerController>();
            experienceSystem ??= GetComponent<ExperienceSystem>();
            weaponSystem ??= GetComponent<WeaponSystem>();
        }

        private void OnEnable()
        {
            if (experienceSystem != null)
            {
                experienceSystem.LevelChanged += OnLevelChanged;
            }
        }

        private void OnDisable()
        {
            if (experienceSystem != null)
            {
                experienceSystem.LevelChanged -= OnLevelChanged;
            }
        }

        public void GrantExperience(float amount)
        {
            experienceSystem?.GainExperience(amount * (_player?.Stats.GetValue(StatType.ExperienceGainMultiplier, 1f) ?? 1f));
        }

        private void OnLevelChanged(int level)
        {
            PrepareDraft();
        }

        private void PrepareDraft()
        {
            _draftBuffer.Clear();
            List<UpgradeData> available = GetAvailableUpgrades();
            if (available.Count == 0)
            {
                DraftReady?.Invoke(Array.Empty<UpgradeData>());
                return;
            }

            int count = Mathf.Clamp(UnityEngine.Random.Range(draftCountMin, draftCountMax + 1), draftCountMin, draftCountMax);
            for (int i = 0; i < count && available.Count > 0; i++)
            {
                UpgradeData selected = DrawWeightedUpgrade(available);
                _draftBuffer.Add(selected);
                available.Remove(selected);
            }

            DraftReady?.Invoke(_draftBuffer);
        }

        private UpgradeData DrawWeightedUpgrade(List<UpgradeData> pool)
        {
            float luck = _player?.Stats.GetValue(StatType.Luck, 0f) ?? 0f;
            var weights = new List<float>(pool.Count);
            foreach (UpgradeData upgrade in pool)
            {
                float weight = 1f;
                if (upgrade.Category == UpgradeCategory.Evolution)
                {
                    weight = 0.2f + luck * 0.05f;
                }
                else if (upgrade.Category == UpgradeCategory.Passive)
                {
                    weight = 1f + luck * 0.1f;
                }
                else
                {
                    weight = 1.2f + luck * 0.05f;
                }

                weights.Add(Mathf.Max(0.01f, weight));
            }

            return WeightedRandomPicker.Pick(pool, weights, UnityEngine.Random.value);
        }

        public void SelectUpgrade(UpgradeData upgrade)
        {
            if (!_upgradeRanks.TryGetValue(upgrade.Id, out int rank))
            {
                rank = 0;
            }

            rank += 1;
            _upgradeRanks[upgrade.Id] = rank;
            ApplyUpgrade(upgrade);
        }

        public void Reroll()
        {
            if (rerolls <= 0)
            {
                return;
            }

            rerolls -= 1;
            PrepareDraft();
            RerollsChanged?.Invoke(rerolls);
        }

        public void Skip()
        {
            if (skips <= 0)
            {
                return;
            }

            skips -= 1;
            SkipsChanged?.Invoke(skips);
        }

        private void ApplyUpgrade(UpgradeData upgrade)
        {
            if (_player == null)
            {
                return;
            }

            switch (upgrade.Category)
            {
                case UpgradeCategory.Weapon:
                    ApplyWeaponUpgrade(upgrade);
                    break;
                case UpgradeCategory.Passive:
                    ApplyStatUpgrade(upgrade);
                    break;
                case UpgradeCategory.Evolution:
                    TryEvolveWeapon(upgrade);
                    break;
            }
        }

        private void ApplyWeaponUpgrade(UpgradeData upgrade)
        {
            if (weaponSystem == null)
            {
                return;
            }

            WeaponData? weapon = Resources.Load<WeaponData>($"Weapons/{upgrade.Id}");
            if (weapon != null)
            {
                weaponSystem.LevelWeapon(weapon);
            }
        }

        private void ApplyStatUpgrade(UpgradeData upgrade)
        {
            StatCollection stats = _player!.Stats;
            foreach (StatModifier modifier in upgrade.StatModifiers)
            {
                stats.ApplyModifier(modifier.Stat, modifier.Operation, modifier.Value);
            }
        }

        private void TryEvolveWeapon(UpgradeData upgrade)
        {
            if (weaponSystem == null)
            {
                return;
            }

            EvolutionRule? rule = Resources.Load<EvolutionRule>($"Evolutions/{upgrade.Id}");
            if (rule == null)
            {
                return;
            }

            if (rule.ResultWeapon != null)
            {
                weaponSystem.EquipWeapon(rule.ResultWeapon, 1);
            }
        }

        private List<UpgradeData> GetAvailableUpgrades()
        {
            var available = new List<UpgradeData>();
            foreach (UpgradeData upgrade in upgradePool)
            {
                int rank = _upgradeRanks.TryGetValue(upgrade.Id, out int r) ? r : 0;
                if (rank < upgrade.MaxRank)
                {
                    available.Add(upgrade);
                }
            }

            return available;
        }
    }
}
