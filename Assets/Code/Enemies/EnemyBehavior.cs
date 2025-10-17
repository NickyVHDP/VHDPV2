#nullable enable
using UnityEngine;
using VHDPV2.Damage;

namespace VHDPV2.Enemies
{
    public interface IEnemyBehavior
    {
        void Initialize(Transform agent, Transform player, EnemyData data);
        void Tick(float deltaTime);
    }

    public abstract class EnemyBehavior : ScriptableObject, IEnemyBehavior
    {
        protected Transform? Agent;
        protected Transform? Player;
        protected EnemyData? Data;

        public virtual void Initialize(Transform agent, Transform player, EnemyData data)
        {
            Agent = agent;
            Player = player;
            Data = data;
        }

        public abstract void Tick(float deltaTime);

        protected static Vector2 DirectionToPlayer(Transform agent, Transform player)
        {
            return (player.position - agent.position).normalized;
        }
    }
}
