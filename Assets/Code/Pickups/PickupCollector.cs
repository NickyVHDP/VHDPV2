#nullable enable
using System.Collections.Generic;
using UnityEngine;

namespace VHDPV2.Pickups
{
    public sealed class PickupCollector : MonoBehaviour
    {
        [SerializeField] private float radius = 2f;
        [SerializeField] private LayerMask pickupMask;
        [SerializeField] private float vacuumSpeed = 10f;

        private readonly List<PickupItem> _buffer = new();
        private float _magnetMultiplier = 1f;

        public void SetMagnetMultiplier(float value)
        {
            _magnetMultiplier = Mathf.Max(0.1f, value);
        }

        private void Update()
        {
            float effectiveRadius = radius * _magnetMultiplier;
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, effectiveRadius, pickupMask);
            foreach (Collider2D hit in hits)
            {
                if (hit.TryGetComponent(out PickupItem pickup) && !_buffer.Contains(pickup))
                {
                    _buffer.Add(pickup);
                }
            }

            for (int i = _buffer.Count - 1; i >= 0; i--)
            {
                PickupItem pickup = _buffer[i];
                if (pickup == null)
                {
                    _buffer.RemoveAt(i);
                    continue;
                }

                pickup.VacuumTowards(transform.position, vacuumSpeed * Time.deltaTime);
                if (Vector2.Distance(pickup.transform.position, transform.position) <= 0.2f)
                {
                    pickup.Collect(gameObject);
                    _buffer.RemoveAt(i);
                }
            }
        }
    }
}
