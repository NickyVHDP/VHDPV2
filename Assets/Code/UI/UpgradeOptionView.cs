#nullable enable
using System;
using UnityEngine;
using UnityEngine.UI;
using VHDPV2.Upgrades;

namespace VHDPV2.UI
{
    public sealed class UpgradeOptionView : MonoBehaviour
    {
        [SerializeField] private TMPro.TMP_Text? title;
        [SerializeField] private Image? icon;
        [SerializeField] private Button? button;

        private UpgradeData? _upgrade;
        private Action? _onSelected;

        public void Bind(UpgradeData upgrade, Action onSelected)
        {
            _upgrade = upgrade;
            _onSelected = onSelected;
            if (title != null)
            {
                title.text = upgrade.DisplayName;
            }

            if (icon != null)
            {
                icon.sprite = upgrade.Icon;
                icon.enabled = upgrade.Icon != null;
            }

            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(HandleClick);
            }
        }

        private void HandleClick()
        {
            _onSelected?.Invoke();
        }
    }
}
