#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;
using VHDPV2.Damage;

namespace VHDPV2.Effects
{
    [RequireComponent(typeof(DamageReceiver))]
    public sealed class StatusEffectController : MonoBehaviour
    {
        private readonly List<StatusEffectInstance> _activeEffects = new();
        private DamageReceiver? _receiver;

        public event Action<StatusEffectType>? StatusEffectApplied;

        private void Awake()
        {
            _receiver = GetComponent<DamageReceiver>();
        }

        private void Update()
        {
            for (int i = _activeEffects.Count - 1; i >= 0; i--)
            {
                StatusEffectInstance effect = _activeEffects[i];
                bool alive = effect.Tick(Time.deltaTime, out float damage);
                if (damage > 0f && _receiver != null)
                {
                    _receiver.ApplyDamageOverTime(damage);
                }

                if (!alive)
                {
                    _activeEffects.RemoveAt(i);
                }
            }
        }

        public void Apply(StatusEffectData data)
        {
            if (UnityEngine.Random.value > data.Chance)
            {
                return;
            }

            _activeEffects.Add(new StatusEffectInstance(data));
            StatusEffectApplied?.Invoke(data.Type);
        }
    }
}
