#nullable enable
using UnityEngine;
using UnityEngine.UI;
using VHDPV2.Player;
using VHDPV2.Systems;

namespace VHDPV2.UI
{
    public sealed class HUDController : MonoBehaviour
    {
        [SerializeField] private Slider? healthBar;
        [SerializeField] private Slider? experienceBar;
        [SerializeField] private TMPro.TMP_Text? levelLabel;
        [SerializeField] private TMPro.TMP_Text? timerLabel;
        [SerializeField] private PlayerController? player;
        [SerializeField] private ExperienceSystem? experienceSystem;

        private float _elapsed;

        private void Awake()
        {
            player ??= FindObjectOfType<PlayerController>();
            experienceSystem ??= player != null ? player.GetComponent<ExperienceSystem>() : null;
        }

        private void OnEnable()
        {
            if (experienceSystem != null)
            {
                experienceSystem.ExperienceChanged += OnExperienceChanged;
                experienceSystem.LevelChanged += OnLevelChanged;
            }
        }

        private void OnDisable()
        {
            if (experienceSystem != null)
            {
                experienceSystem.ExperienceChanged -= OnExperienceChanged;
                experienceSystem.LevelChanged -= OnLevelChanged;
            }
        }

        private void Update()
        {
            _elapsed += Time.deltaTime;
            if (timerLabel != null)
            {
                int minutes = Mathf.FloorToInt(_elapsed / 60f);
                int seconds = Mathf.FloorToInt(_elapsed % 60f);
                timerLabel.text = $"{minutes:00}:{seconds:00}";
            }

            if (player != null && healthBar != null)
            {
                var receiver = player.GetComponent<VHDPV2.Damage.DamageReceiver>();
                if (receiver != null)
                {
                    healthBar.value = receiver.CurrentHealth / receiver.MaxHealth;
                }
            }
        }

        private void OnExperienceChanged(float current, float target)
        {
            if (experienceBar != null)
            {
                experienceBar.value = current / Mathf.Max(target, 0.001f);
            }
        }

        private void OnLevelChanged(int level)
        {
            if (levelLabel != null)
            {
                levelLabel.text = $"Lv. {level}";
            }
        }
    }
}
