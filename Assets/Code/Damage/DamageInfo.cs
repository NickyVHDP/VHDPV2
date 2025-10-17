#nullable enable
using VHDPV2.Effects;

namespace VHDPV2.Damage
{
    public readonly struct DamageInfo
    {
        public readonly float BaseDamage;
        public readonly float CritChance;
        public readonly float CritMultiplier;
        public readonly float ArmorPenetration;
        public readonly StatusEffectData? StatusEffect;

        public DamageInfo(float baseDamage, float critChance, float critMultiplier, float armorPenetration, StatusEffectData? statusEffect)
        {
            BaseDamage = baseDamage;
            CritChance = critChance;
            CritMultiplier = critMultiplier;
            ArmorPenetration = armorPenetration;
            StatusEffect = statusEffect;
        }
    }
}
