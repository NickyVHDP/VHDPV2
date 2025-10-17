#nullable enable
using UnityEngine;
using VHDPV2.Pickups;
using VHDPV2.Systems;

namespace VHDPV2.Enemies
{
    [RequireComponent(typeof(EnemyAIController))]
    [RequireComponent(typeof(VHDPV2.Damage.DamageReceiver))]
    public sealed class LootDropper : MonoBehaviour
    {
        [SerializeField] private EnemyData? data;

        private VHDPV2.Damage.DamageReceiver? _receiver;

        private void Awake()
        {
            _receiver = GetComponent<VHDPV2.Damage.DamageReceiver>();
        }

        private void OnEnable()
        {
            if (_receiver != null)
            {
                _receiver.Died += HandleDeath;
            }
        }

        private void OnDisable()
        {
            if (_receiver != null)
            {
                _receiver.Died -= HandleDeath;
            }
        }

        public void SetData(EnemyData enemyData)
        {
            data = enemyData;
        }

        private void HandleDeath()
        {
            if (data == null)
            {
                return;
            }

            if (Core.GameServiceLocator.TryGet(out GameSessionManager? session))
            {
                session.RegisterKill(data.Id);
            }

            PickupItem? pickup = data.LootTable.Roll();
            if (pickup != null)
            {
                Instantiate(pickup.gameObject, transform.position, Quaternion.identity);
            }
        }
    }
}
