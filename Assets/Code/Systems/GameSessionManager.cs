#nullable enable
using System.Collections.Generic;
using UnityEngine;
using VHDPV2.UI;

namespace VHDPV2.Systems
{
    public sealed class GameSessionManager : MonoBehaviour
    {
        [SerializeField] private ResultsScreen? resultsScreen;
        [SerializeField] private float sessionDuration = 600f;

        private readonly Dictionary<string, int> _kills = new();
        private float _elapsed;
        private bool _sessionActive;

        private void Awake()
        {
            Core.GameServiceLocator.Register(this);
        }

        private void OnDestroy()
        {
            Core.GameServiceLocator.Unregister<GameSessionManager>();
        }

        private void OnEnable()
        {
            _sessionActive = true;
        }

        private void Update()
        {
            if (!_sessionActive)
            {
                return;
            }

            _elapsed += Time.deltaTime;
            if (_elapsed >= sessionDuration)
            {
                EndSession();
            }
        }

        public void RegisterKill(string enemyId)
        {
            if (_kills.TryGetValue(enemyId, out int value))
            {
                _kills[enemyId] = value + 1;
            }
            else
            {
                _kills[enemyId] = 1;
            }
        }

        public void EndSession()
        {
            if (!_sessionActive)
            {
                return;
            }

            _sessionActive = false;
            if (resultsScreen != null)
            {
                foreach (var kvp in _kills)
                {
                    resultsScreen.RecordKill(kvp.Key);
                }

                resultsScreen.Show(_elapsed);
            }
        }
    }
}
