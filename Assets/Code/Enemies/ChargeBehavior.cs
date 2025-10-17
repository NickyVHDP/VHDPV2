#nullable enable
using UnityEngine;

namespace VHDPV2.Enemies
{
    [CreateAssetMenu(fileName = "ChargeBehavior", menuName = "VHD/Enemy Behaviors/Charge")]
    public sealed class ChargeBehavior : EnemyBehavior
    {
        [SerializeField] private float windupTime = 1f;
        [SerializeField] private float chargeSpeedMultiplier = 4f;
        [SerializeField] private float cooldown = 2f;

        private float _timer;
        private Vector2 _chargeDirection;
        private bool _charging;

        public override void Initialize(Transform agent, Transform player, EnemyData data)
        {
            base.Initialize(agent, player, data);
            _timer = windupTime;
        }

        public override void Tick(float deltaTime)
        {
            if (Agent == null || Player == null || Data == null)
            {
                return;
            }

            _timer -= deltaTime;
            if (_charging)
            {
                Agent.position += (Vector3)(_chargeDirection * Data.Speed * chargeSpeedMultiplier * deltaTime);
                if (_timer <= 0f)
                {
                    _charging = false;
                    _timer = cooldown;
                }

                return;
            }

            if (_timer <= 0f)
            {
                _charging = true;
                _timer = windupTime;
                _chargeDirection = DirectionToPlayer(Agent, Player);
            }
            else
            {
                Vector2 direction = DirectionToPlayer(Agent, Player);
                Agent.position += (Vector3)(direction * Data.Speed * deltaTime);
            }
        }
    }
}
