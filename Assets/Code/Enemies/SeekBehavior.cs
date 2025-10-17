#nullable enable
using UnityEngine;

namespace VHDPV2.Enemies
{
    [CreateAssetMenu(fileName = "SeekBehavior", menuName = "VHD/Enemy Behaviors/Seek")]
    public sealed class SeekBehavior : EnemyBehavior
    {
        [SerializeField] private float acceleration = 5f;

        private Vector2 _velocity;

        public override void Tick(float deltaTime)
        {
            if (Agent == null || Player == null || Data == null)
            {
                return;
            }

            Vector2 direction = DirectionToPlayer(Agent, Player);
            _velocity = Vector2.MoveTowards(_velocity, direction * Data.Speed, acceleration * deltaTime);
            Agent.position += (Vector3)(_velocity * deltaTime);
        }
    }
}
