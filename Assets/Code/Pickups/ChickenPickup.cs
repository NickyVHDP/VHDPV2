#nullable enable
using UnityEngine;
using VHDPV2.Damage;

namespace VHDPV2.Pickups
{
    public sealed class ChickenPickup : PickupItem
    {
        [SerializeField] private float healAmount = 20f;

        public override void Collect(GameObject collector)
        {
            if (collector.TryGetComponent(out DamageReceiver receiver))
            {
                receiver.Heal(healAmount);
            }

            Destroy(gameObject);
        }
    }
}
