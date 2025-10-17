#nullable enable
using UnityEngine;
using VHDPV2.Damage;

namespace VHDPV2.Weapons
{
    [RequireComponent(typeof(DamageDealer))]
    public sealed class Projectile : MonoBehaviour
    {
        [SerializeField] private float lifetime = 2f;
        [SerializeField] private float speed = 10f;
        [SerializeField] private bool pierce;
        [SerializeField] private GameObject? poolPrefab;

        private DamageDealer? _damageDealer;
        private float _timeAlive;
        private Vector2 _direction;

        private void Awake()
        {
            _damageDealer = GetComponent<DamageDealer>();
        }

        public void Launch(Vector2 direction, float overrideSpeed, float overrideLifetime)
        {
            _direction = direction.normalized;
            speed = overrideSpeed;
            lifetime = overrideLifetime;
            _timeAlive = 0f;
        }

        private void Update()
        {
            transform.position += (Vector3)(_direction * speed * Time.deltaTime);
            _timeAlive += Time.deltaTime;
            if (_timeAlive >= lifetime)
            {
                Despawn();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.TryGetComponent(out DamageReceiver receiver))
            {
                return;
            }

            _damageDealer?.DealDamage(receiver);
            if (!pierce)
            {
                Despawn();
            }
        }

        public void SetPoolPrefab(GameObject prefab)
        {
            poolPrefab = prefab;
        }

        private void Despawn()
        {
            if (poolPrefab != null)
            {
                Core.GameServiceLocator.Require<Core.ObjectPoolService>().Release(poolPrefab, gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
