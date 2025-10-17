#nullable enable
using System.Collections.Generic;
using UnityEngine;
using VHDPV2.Core;
using VHDPV2.Damage;

namespace VHDPV2.Weapons
{
    [CreateAssetMenu(fileName = "ProjectileWeaponBehavior", menuName = "VHD/Weapons/Projectile Behavior")]
    public sealed class ProjectileWeaponBehavior : WeaponBehavior
    {
        [SerializeField] private GameObject? projectilePrefab;
        [SerializeField] private float spawnRadius = 0.5f;

        private WeaponRuntimeContext _context;
        private float _cooldownTimer;
        private int _currentLevel;
        private readonly List<Transform> _targets = new();

        public override void Initialize(WeaponRuntimeContext context)
        {
            _context = context;
            if (projectilePrefab != null)
            {
                context.Pool.WarmPool(projectilePrefab, 8);
            }
        }

        public override void Tick(float deltaTime)
        {
            if (projectilePrefab == null)
            {
                return;
            }

            _cooldownTimer -= deltaTime * _context.Stats.GetValue(StatType.CooldownMultiplier, 1f);
            if (_cooldownTimer > 0f)
            {
                return;
            }

            FireProjectiles();
        }

        public override void OnLevelChanged(int level)
        {
            _currentLevel = level;
        }

        private void FireProjectiles()
        {
            if (projectilePrefab == null)
            {
                return;
            }

            int projectiles = _context.Data.Projectiles + CalculateProjectileBonus(_currentLevel);
            float speed = _context.Data.Speed * _context.Stats.GetValue(StatType.ProjectileSpeed, 1f);
            float lifetime = _context.Data.Lifetime * _context.Stats.GetValue(StatType.Area, 1f);

            for (int i = 0; i < projectiles; i++)
            {
                Vector2 direction = SelectDirection();
                SpawnProjectile(direction, speed, lifetime);
            }

            float attackSpeed = Mathf.Max(0.1f, _context.Stats.GetValue(StatType.AttackSpeedMultiplier, 1f));
            _cooldownTimer = Mathf.Max(0.1f, _context.Data.Cooldown / attackSpeed);
        }

        private int CalculateProjectileBonus(int level)
        {
            int bonus = 0;
            foreach (WeaponLevelModifier modifier in _context.Data.ScalingPerLevel)
            {
                if (level >= modifier.Level)
                {
                    bonus += modifier.AdditionalProjectiles;
                }
            }

            return bonus;
        }

        private Vector2 SelectDirection()
        {
            Transform? target = AcquireTarget();
            if (target == null)
            {
                return Random.insideUnitCircle.normalized;
            }

            return (target.position - _context.Origin.position).normalized;
        }

        private Transform? AcquireTarget()
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(_context.Origin.position, 25f, _context.TargetMask);
            if (hits.Length == 0)
            {
                return null;
            }

            switch (_context.Data.Targeting)
            {
                case WeaponTargeting.Nearest:
                    return GetNearest(hits);
                case WeaponTargeting.Random:
                    return hits[Random.Range(0, hits.Length)].transform;
                case WeaponTargeting.Furthest:
                    return GetFurthest(hits);
                case WeaponTargeting.ThreatWeighted:
                    return GetWeightedTarget(hits);
                default:
                    return GetNearest(hits);
            }
        }

        private Transform GetNearest(IReadOnlyList<Collider2D> hits)
        {
            Transform nearest = hits[0].transform;
            float bestDistance = float.MaxValue;
            foreach (Collider2D hit in hits)
            {
                float distance = Vector2.Distance(hit.transform.position, _context.Origin.position);
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    nearest = hit.transform;
                }
            }

            return nearest;
        }

        private Transform GetFurthest(IReadOnlyList<Collider2D> hits)
        {
            Transform furthest = hits[0].transform;
            float bestDistance = 0f;
            foreach (Collider2D hit in hits)
            {
                float distance = Vector2.Distance(hit.transform.position, _context.Origin.position);
                if (distance > bestDistance)
                {
                    bestDistance = distance;
                    furthest = hit.transform;
                }
            }

            return furthest;
        }

        private Transform GetWeightedTarget(IReadOnlyList<Collider2D> hits)
        {
            _targets.Clear();
            foreach (Collider2D hit in hits)
            {
                _targets.Add(hit.transform);
            }

            int index = Random.Range(0, _targets.Count);
            return _targets[index];
        }

        private void SpawnProjectile(Vector2 direction, float speed, float lifetime)
        {
            if (projectilePrefab == null)
            {
                return;
            }

            GameObject projectile = _context.Pool.Get(projectilePrefab);
            projectile.transform.position = _context.Origin.position + (Vector3)(direction * spawnRadius);
            if (!projectile.TryGetComponent(out Projectile component))
            {
                component = projectile.AddComponent<Projectile>();
            }

            component.SetPoolPrefab(projectilePrefab);
            component.Launch(direction, speed, lifetime);

            if (projectile.TryGetComponent(out DamageDealer dealer))
            {
                float damage = _context.Data.BaseDamage * _context.Stats.GetValue(StatType.DamageMultiplier, 1f);
                float critChance = _context.Stats.GetValue(StatType.CritChance, 0f);
                float critMultiplier = _context.Stats.GetValue(StatType.CritMultiplier, 2f);
                var info = new DamageInfo(damage, critChance, critMultiplier, 0f, _context.Data.StatusOnHit);
                dealer.Configure(info);
            }
        }
    }
}
