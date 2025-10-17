#nullable enable
using UnityEngine;
using VHDPV2.Core;
using VHDPV2.Damage;

namespace VHDPV2.Weapons
{
    public abstract class WeaponBehavior : ScriptableObject
    {
        public abstract void Initialize(WeaponRuntimeContext context);
        public abstract void Tick(float deltaTime);
        public abstract void OnLevelChanged(int level);
    }

    public readonly struct WeaponRuntimeContext
    {
        public readonly Transform Origin;
        public readonly WeaponData Data;
        public readonly StatCollection Stats;
        public readonly ObjectPoolService Pool;
        public readonly LayerMask TargetMask;

        public WeaponRuntimeContext(Transform origin, WeaponData data, StatCollection stats, ObjectPoolService pool, LayerMask targetMask)
        {
            Origin = origin;
            Data = data;
            Stats = stats;
            Pool = pool;
            TargetMask = targetMask;
        }
    }
}
