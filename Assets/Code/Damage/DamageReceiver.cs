#nullable enable
using System;
using UnityEngine;
using VHDPV2.Effects;
using VHDPV2.Systems;
using VHDPV2.Utils;

namespace VHDPV2.Damage
{
    public sealed class DamageReceiver : MonoBehaviour
    {
        [SerializeField] private CombatStats stats = new();
        [SerializeField] private bool clampArmor = true;

        private bool _isInvulnerable;
        private float _damageOverTimeBuffer;

        public float CurrentHealth { get; private set; }
        public float MaxHealth => stats.Health;

        public event Action<float>? Damaged;
        public event Action? Died;

        private void Awake()
        {
            CurrentHealth = stats.Health;
        }

        private void Update()
        {
            if (_damageOverTimeBuffer > 0f)
            {
                ApplyDamageInternal(_damageOverTimeBuffer * Time.deltaTime, false);
            }
        }

        public void ResetHealth(float newMax)
        {
            stats.Health = newMax;
            CurrentHealth = Mathf.Min(CurrentHealth, stats.Health);
        }

        public void RestoreToFull()
        {
            CurrentHealth = stats.Health;
        }

        public void Heal(float amount)
        {
            if (amount <= 0f)
            {
                return;
            }

            CurrentHealth = Mathf.Min(CurrentHealth + amount, stats.Health);
        }

        public void SetInvulnerable(bool invulnerable)
        {
            _isInvulnerable = invulnerable;
        }

        public void ApplyDamage(DamageInfo info)
        {
            if (_isInvulnerable)
            {
                return;
            }

            float damage = CalculateDamage(info.BaseDamage, info.CritChance, info.CritMultiplier, info.ArmorPenetration);
            ApplyDamageInternal(damage, true);

            if (info.StatusEffect != null && TryGetComponent(out StatusEffectController controller))
            {
                controller.Apply(info.StatusEffect);
            }
        }

        public void ApplyDamageOverTime(float dps)
        {
            _damageOverTimeBuffer = Mathf.Max(0f, dps);
        }

        private float CalculateDamage(float baseDamage, float critChance, float critMultiplier, float armorPenetration)
        {
            float critRoll = UnityEngine.Random.value;
            float damage = baseDamage;
            if (critRoll <= critChance)
            {
                damage *= critMultiplier;
            }

            float effectiveArmor = Mathf.Max(0f, stats.Armor - armorPenetration);
            if (!clampArmor)
            {
                effectiveArmor = Mathf.Max(0f, effectiveArmor);
            }

            float reduction = CombatMath.CalculateArmorReduction(effectiveArmor);
            return damage * (1f - reduction);
        }

        private void ApplyDamageInternal(float amount, bool triggerEvents)
        {
            if (amount <= 0f)
            {
                return;
            }

            CurrentHealth -= amount;
            if (triggerEvents)
            {
                Damaged?.Invoke(amount);
            }

            if (CurrentHealth <= 0f)
            {
                CurrentHealth = 0f;
                Died?.Invoke();
            }
        }
    }
}
