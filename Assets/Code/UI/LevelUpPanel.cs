#nullable enable
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VHDPV2.Upgrades;

namespace VHDPV2.UI
{
    public sealed class LevelUpPanel : MonoBehaviour
    {
        [SerializeField] private GameObject? optionPrefab;
        [SerializeField] private Transform? optionRoot;
        [SerializeField] private LevelUpSystem? levelUpSystem;

        private readonly List<GameObject> _spawnedOptions = new();

        private void Awake()
        {
            levelUpSystem ??= FindObjectOfType<LevelUpSystem>();
        }

        private void OnEnable()
        {
            if (levelUpSystem != null)
            {
                levelUpSystem.DraftReady += OnDraftReady;
            }
        }

        private void OnDisable()
        {
            if (levelUpSystem != null)
            {
                levelUpSystem.DraftReady -= OnDraftReady;
            }
        }

        private void OnDraftReady(IReadOnlyList<UpgradeData> upgrades)
        {
            ClearOptions();
            if (optionPrefab == null || optionRoot == null)
            {
                return;
            }

            foreach (UpgradeData upgrade in upgrades)
            {
                GameObject option = Instantiate(optionPrefab, optionRoot);
                _spawnedOptions.Add(option);
                if (option.TryGetComponent(out UpgradeOptionView view))
                {
                    view.Bind(upgrade, () => HandleSelection(upgrade));
                }
            }

            gameObject.SetActive(true);
        }

        private void HandleSelection(UpgradeData upgrade)
        {
            levelUpSystem?.SelectUpgrade(upgrade);
            gameObject.SetActive(false);
        }

        public void Reroll()
        {
            levelUpSystem?.Reroll();
        }

        public void Skip()
        {
            levelUpSystem?.Skip();
            gameObject.SetActive(false);
        }

        private void ClearOptions()
        {
            foreach (GameObject option in _spawnedOptions)
            {
                if (option != null)
                {
                    Destroy(option);
                }
            }

            _spawnedOptions.Clear();
        }
    }
}
