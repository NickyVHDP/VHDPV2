#nullable enable
using UnityEngine;
using VHDPV2.Save;

namespace VHDPV2.Pickups
{
    public sealed class CoinPickup : PickupItem
    {
        [SerializeField] private int amount = 1;

        public override void Collect(GameObject collector)
        {
            if (Core.GameServiceLocator.TryGet(out MetaShopService? service))
            {
                service.AddGold(amount);
            }

            Destroy(gameObject);
        }
    }
}
