#nullable enable
using UnityEngine;

namespace VHDPV2.Pickups
{
    public abstract class PickupItem : MonoBehaviour
    {
        [SerializeField] private float vacuumAcceleration = 30f;
        private Vector3 _velocity;

        public void VacuumTowards(Vector3 target, float step)
        {
            Vector3 direction = (target - transform.position).normalized;
            _velocity += direction * vacuumAcceleration * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target, step + _velocity.magnitude * Time.deltaTime);
        }

        public abstract void Collect(GameObject collector);
    }
}
