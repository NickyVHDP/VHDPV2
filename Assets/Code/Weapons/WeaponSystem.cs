#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;
using VHDPV2.Core;
using VHDPV2.Player;

namespace VHDPV2.Weapons
{
    public sealed class WeaponSystem : MonoBehaviour
    {
        [Serializable]
        private sealed class EquippedWeapon
        {
            public WeaponData? Data;
            public int Level = 1;
            public WeaponBehavior? BehaviorInstance;
        }

        [SerializeField] private List<WeaponData> startingWeapons = new();
        [SerializeField] private LayerMask enemyMask;

        private readonly List<EquippedWeapon> _weapons = new();
        private PlayerController? _player;
        private ObjectPoolService? _poolService;

        private void Awake()
        {
            _player = GetComponent<PlayerController>();
            _poolService = GameServiceLocator.Require<ObjectPoolService>();
        }

        private void Start()
        {
            foreach (WeaponData data in startingWeapons)
            {
                EquipWeapon(data, 1);
            }
        }

        private void Update()
        {
            if (_player == null)
            {
                return;
            }

            foreach (EquippedWeapon weapon in _weapons)
            {
                if (weapon.Data == null || weapon.BehaviorInstance == null)
                {
                    continue;
                }

                weapon.BehaviorInstance.Tick(Time.deltaTime);
            }
        }

        public void EquipWeapon(WeaponData data, int level)
        {
            if (_player == null)
            {
                return;
            }

            var existing = _weapons.Find(w => w.Data == data);
            if (existing != null)
            {
                existing.Level = Mathf.Max(existing.Level, level);
                existing.BehaviorInstance?.OnLevelChanged(existing.Level);
                return;
            }

            WeaponBehavior? behavior = data.Behavior == null ? null : Instantiate(data.Behavior);
            if (behavior != null && _poolService != null)
            {
                var context = new WeaponRuntimeContext(_player.transform, data, _player.Stats, _poolService, enemyMask);
                behavior.Initialize(context);
                behavior.OnLevelChanged(level);
            }

            _weapons.Add(new EquippedWeapon
            {
                Data = data,
                Level = level,
                BehaviorInstance = behavior
            });
        }

        public void LevelWeapon(WeaponData data)
        {
            var weapon = _weapons.Find(w => w.Data == data);
            if (weapon == null)
            {
                EquipWeapon(data, 1);
                return;
            }

            weapon.Level += 1;
            weapon.BehaviorInstance?.OnLevelChanged(weapon.Level);
        }

        public IReadOnlyList<WeaponData> GetEquippedWeapons()
        {
            var result = new List<WeaponData>();
            foreach (EquippedWeapon weapon in _weapons)
            {
                if (weapon.Data != null)
                {
                    result.Add(weapon.Data);
                }
            }

            return result;
        }
    }
}
