#nullable enable
using UnityEngine;

namespace VHDPV2.Enemies
{
    [CreateAssetMenu(fileName = "KiteBehavior", menuName = "VHD/Enemy Behaviors/Kite")]
    public sealed class KiteBehavior : EnemyBehavior
    {
        [SerializeField] private float preferredDistance = 6f;
        [SerializeField] private float repositionSpeed = 4f;

        public override void Tick(float deltaTime)
        {
            if (Agent == null || Player == null || Data == null)
            {
                return;
            }

            float distance = Vector2.Distance(Agent.position, Player.position);
            Vector2 direction = DirectionToPlayer(Agent, Player);
            if (distance < preferredDistance)
            {
                Agent.position -= (Vector3)(direction * Data.Speed * deltaTime);
            }
            else if (distance > preferredDistance + 1f)
            {
                Agent.position += (Vector3)(direction * Data.Speed * deltaTime);
            }
            else
            {
                Vector2 strafe = new(-direction.y, direction.x);
                Agent.position += (Vector3)(strafe * repositionSpeed * deltaTime);
            }
        }
    }
}
