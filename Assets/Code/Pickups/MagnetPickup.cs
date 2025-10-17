#nullable enable
using UnityEngine;

namespace VHDPV2.Pickups
{
    public sealed class MagnetPickup : PickupItem
    {
        [SerializeField] private float radius = 50f;
        [SerializeField] private LayerMask pickupMask;

        public override void Collect(GameObject collector)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(collector.transform.position, radius, pickupMask);
            foreach (Collider2D hit in hits)
            {
                if (hit.TryGetComponent(out PickupItem pickup) && pickup != this)
                {
                    pickup.Collect(collector);
                }
            }

            Destroy(gameObject);
        }
    }
}
