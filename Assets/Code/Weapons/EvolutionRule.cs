#nullable enable
using UnityEngine;

namespace VHDPV2.Weapons
{
    [CreateAssetMenu(fileName = "EvolutionRule", menuName = "VHD/Weapons/Evolution Rule")]
    public sealed class EvolutionRule : ScriptableObject
    {
        [SerializeField] private WeaponData? requiresWeapon;
        [SerializeField] private PassiveData? requiresPassive;
        [SerializeField] private int minWeaponRank = 8;
        [SerializeField] private int minPassiveRank = 5;
        [SerializeField] private WeaponData? resultWeapon;

        public WeaponData? RequiresWeapon => requiresWeapon;
        public PassiveData? RequiresPassive => requiresPassive;
        public int MinWeaponRank => minWeaponRank;
        public int MinPassiveRank => minPassiveRank;
        public WeaponData? ResultWeapon => resultWeapon;
    }
}
