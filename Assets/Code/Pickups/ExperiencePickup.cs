#nullable enable
using UnityEngine;
using VHDPV2.Upgrades;

namespace VHDPV2.Pickups
{
    public sealed class ExperiencePickup : PickupItem
    {
        [SerializeField] private float amount = 10f;

        public override void Collect(GameObject collector)
        {
            var levelUp = collector.GetComponent<LevelUpSystem>();
            levelUp?.GrantExperience(amount);
            Destroy(gameObject);
        }
    }
}
