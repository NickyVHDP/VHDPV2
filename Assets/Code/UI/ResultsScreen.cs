#nullable enable
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace VHDPV2.UI
{
    public sealed class ResultsScreen : MonoBehaviour
    {
        [SerializeField] private TMP_Text? durationLabel;
        [SerializeField] private TMP_Text? killsLabel;
        [SerializeField] private TMP_Text? lootLabel;

        private float _duration;
        private readonly Dictionary<string, int> _kills = new();
        private readonly List<string> _loot = new();

        public void RecordKill(string enemyId)
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

        public void RecordLoot(string lootId)
        {
            _loot.Add(lootId);
        }

        public void Show(float duration)
        {
            _duration = duration;
            if (durationLabel != null)
            {
                int minutes = Mathf.FloorToInt(duration / 60f);
                int seconds = Mathf.FloorToInt(duration % 60f);
                durationLabel.text = $"Time: {minutes:00}:{seconds:00}";
            }

            if (killsLabel != null)
            {
                killsLabel.text = string.Join("\n", _kills.Select(kvp => $"{kvp.Key}: {kvp.Value}"));
            }

            if (lootLabel != null)
            {
                lootLabel.text = string.Join(", ", _loot);
            }

            gameObject.SetActive(true);
        }
    }
}
