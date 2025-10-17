#nullable enable
using UnityEngine;
using VHDPV2.Effects;

namespace VHDPV2.Damage
{
    public sealed class DamageDealer : MonoBehaviour
    {
        [SerializeField] private float baseDamage = 5f;
        [SerializeField] private float critChance = 0.05f;
        [SerializeField] private float critMultiplier = 2f;
        [SerializeField] private float armorPenetration;
        [SerializeField] private StatusEffectData? onHitStatus;

        private DamageInfo _runtimeInfo;

        private void Awake()
        {
            _runtimeInfo = new DamageInfo(baseDamage, critChance, critMultiplier, armorPenetration, onHitStatus);
        }

        public void Configure(DamageInfo info)
        {
            _runtimeInfo = info;
        }

        public void ResetConfiguration()
        {
            _runtimeInfo = new DamageInfo(baseDamage, critChance, critMultiplier, armorPenetration, onHitStatus);
        }

        public void DealDamage(DamageReceiver receiver)
        {
            receiver.ApplyDamage(_runtimeInfo);
        }
    }
}
