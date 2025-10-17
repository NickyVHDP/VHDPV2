#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;
using VHDPV2.Effects;

namespace VHDPV2.Weapons
{
    public enum WeaponTargeting
    {
        Nearest,
        Random,
        Furthest,
        ThreatWeighted
    }

    [CreateAssetMenu(fileName = "WeaponData", menuName = "VHD/Weapon Data")]
    public sealed class WeaponData : ScriptableObject
    {
        [SerializeField] private string id = string.Empty;
        [SerializeField] private WeaponBehavior? behavior;
        [SerializeField] private WeaponTargeting targeting = WeaponTargeting.Nearest;
        [SerializeField] private float baseDamage = 10f;
        [SerializeField] private float cooldown = 1f;
        [SerializeField] private int projectiles = 1;
        [SerializeField] private int pierce = 0;
        [SerializeField] private float area = 1f;
        [SerializeField] private float speed = 10f;
        [SerializeField] private float lifetime = 2f;
        [SerializeField] private StatusEffectData? statusOnHit;
        [SerializeField] private List<WeaponLevelModifier> scalingPerLevel = new();

        public string Id => id;
        public WeaponBehavior? Behavior => behavior;
        public WeaponTargeting Targeting => targeting;
        public float BaseDamage => baseDamage;
        public float Cooldown => cooldown;
        public int Projectiles => projectiles;
        public int Pierce => pierce;
        public float Area => area;
        public float Speed => speed;
        public float Lifetime => lifetime;
        public StatusEffectData? StatusOnHit => statusOnHit;
        public IReadOnlyList<WeaponLevelModifier> ScalingPerLevel => scalingPerLevel;
    }

    [Serializable]
    public sealed class WeaponLevelModifier
    {
        public int Level;
        public float DamageBonus;
        public int AdditionalProjectiles;
        public float CooldownMultiplier = 1f;
    }
}
