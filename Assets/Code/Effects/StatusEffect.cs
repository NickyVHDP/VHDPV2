#nullable enable
using System;
using UnityEngine;

namespace VHDPV2.Effects
{
    public enum StatusEffectType
    {
        Burn,
        Slow,
        Freeze,
        Shock,
        Poison,
        Knockback,
        Stun,
        Bleed
    }

    [Serializable]
    public sealed class StatusEffectData
    {
        public StatusEffectType Type;
        public float Duration = 1f;
        public float Magnitude = 1f;
        public float TickInterval = 1f;
        public float Chance = 1f;
        public float DamagePerSecond;
    }

    public interface IStatusEffectListener
    {
        void OnStatusApplied(StatusEffectType type, float duration, float magnitude);
    }

    public sealed class StatusEffectInstance
    {
        public StatusEffectType Type { get; }
        public float RemainingDuration { get; private set; }
        public float Magnitude { get; }
        public float TickInterval { get; }
        public float DamagePerSecond { get; }

        private float _tickTimer;

        public StatusEffectInstance(StatusEffectData data)
        {
            Type = data.Type;
            RemainingDuration = data.Duration;
            Magnitude = data.Magnitude;
            TickInterval = Mathf.Max(0.05f, data.TickInterval);
            DamagePerSecond = data.DamagePerSecond;
            _tickTimer = TickInterval;
        }

        public bool Tick(float deltaTime, out float damage)
        {
            damage = 0f;
            RemainingDuration -= deltaTime;
            _tickTimer -= deltaTime;
            if (_tickTimer <= 0f)
            {
                _tickTimer += TickInterval;
                damage = DamagePerSecond * TickInterval;
            }

            return RemainingDuration > 0f;
        }
    }
}
