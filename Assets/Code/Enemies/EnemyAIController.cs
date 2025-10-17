#nullable enable
using UnityEngine;
using VHDPV2.Damage;

namespace VHDPV2.Enemies
{
    [RequireComponent(typeof(DamageDealer))]
    [RequireComponent(typeof(Damage.DamageReceiver))]
    public sealed class EnemyAIController : MonoBehaviour
    {
        [SerializeField] private EnemyData? data;
        [SerializeField] private EnemyBehavior? seekBehavior;
        [SerializeField] private EnemyBehavior? chargeBehavior;
        [SerializeField] private EnemyBehavior? kiteBehavior;

        private Transform? _player;
        private IEnemyBehavior? _activeBehavior;
        private DamageDealer? _damageDealer;
        private Damage.DamageReceiver? _damageReceiver;

        private void Awake()
        {
            _damageDealer = GetComponent<DamageDealer>();
            _damageReceiver = GetComponent<Damage.DamageReceiver>();
        }

        private void Start()
        {
            var playerObject = GameObject.FindWithTag("Player");
            _player = playerObject != null ? playerObject.transform : null;
            InitializeBehavior();
            if (data != null && _damageReceiver != null)
            {
                _damageReceiver.ResetHealth(data.Health);
                _damageReceiver.RestoreToFull();
            }

            if (TryGetComponent(out LootDropper dropper) && data != null)
            {
                dropper.SetData(data);
            }
        }

        private void Update()
        {
            _activeBehavior?.Tick(Time.deltaTime);
        }

        private void InitializeBehavior()
        {
            if (data == null || _player == null)
            {
                return;
            }

            EnemyBehavior? behavior = data.BehaviorType switch
            {
                EnemyBehaviorType.Seek => seekBehavior,
                EnemyBehaviorType.Charge => chargeBehavior,
                EnemyBehaviorType.Kite => kiteBehavior,
                _ => seekBehavior
            };

            if (behavior == null)
            {
                return;
            }

            behavior = Instantiate(behavior);
            behavior.Initialize(transform, _player, data);
            _activeBehavior = behavior;

            if (_damageDealer != null)
            {
                var info = new DamageInfo(data.Damage, 0f, 1f, 0f, data.OnHitEffect);
                _damageDealer.Configure(info);
            }
        }
    }
}
