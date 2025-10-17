#nullable enable
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VHDPV2.Save;

namespace VHDPV2.UI
{
    public sealed class MetaShopController : MonoBehaviour
    {
        [SerializeField] private TMP_Text? goldLabel;
        [SerializeField] private Transform? itemRoot;
        [SerializeField] private GameObject? itemPrefab;
        [SerializeField] private List<MetaShopItemDefinition> items = new();

        private MetaShopService? _service;

        private void Awake()
        {
            _service = FindObjectOfType<MetaShopService>();
        }

        private void OnEnable()
        {
            Refresh();
        }

        public void Refresh()
        {
            if (_service == null)
            {
                return;
            }

            if (goldLabel != null)
            {
                goldLabel.text = _service.Data.Gold.ToString();
            }

            if (itemRoot == null || itemPrefab == null)
            {
                return;
            }

            foreach (Transform child in itemRoot)
            {
                Destroy(child.gameObject);
            }

            foreach (MetaShopItemDefinition item in items)
            {
                GameObject entry = Instantiate(itemPrefab, itemRoot);
                if (entry.TryGetComponent(out MetaShopItemView view))
                {
                    view.Bind(item, _service, this);
                }
            }
        }
    }

    [System.Serializable]
    public sealed class MetaShopItemDefinition
    {
        public string Id = string.Empty;
        public string DisplayName = string.Empty;
        public int Cost;
        public string Description = string.Empty;
    }

    public sealed class MetaShopItemView : MonoBehaviour
    {
        [SerializeField] private TMP_Text? nameLabel;
        [SerializeField] private TMP_Text? costLabel;
        [SerializeField] private TMP_Text? descriptionLabel;
        [SerializeField] private Button? buyButton;

        private MetaShopItemDefinition? _definition;
        private MetaShopService? _service;
        private MetaShopController? _controller;

        public void Bind(MetaShopItemDefinition definition, MetaShopService service, MetaShopController controller)
        {
            _definition = definition;
            _service = service;
            _controller = controller;
            if (nameLabel != null)
            {
                nameLabel.text = definition.DisplayName;
            }

            if (costLabel != null)
            {
                costLabel.text = definition.Cost.ToString();
            }

            if (descriptionLabel != null)
            {
                descriptionLabel.text = definition.Description;
            }

            if (buyButton != null)
            {
                buyButton.onClick.RemoveAllListeners();
                buyButton.onClick.AddListener(Buy);
            }
        }

        private void Buy()
        {
            if (_definition == null || _service == null)
            {
                return;
            }

            if (_service.Data.Gold < _definition.Cost)
            {
                return;
            }

            _service.Data.Gold -= _definition.Cost;
            _service.Purchase(_definition.Id);
            _controller?.Refresh();
        }
    }
}
