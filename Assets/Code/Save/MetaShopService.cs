#nullable enable
using System.IO;
using UnityEngine;

namespace VHDPV2.Save
{
    public sealed class MetaShopService : MonoBehaviour
    {
        [SerializeField] private string fileName = "meta_shop.json";
        private MetaShopSaveData _data = new();

        public MetaShopSaveData Data => _data;

        private void Awake()
        {
            Load();
            Core.GameServiceLocator.Register(this);
        }

        private void OnDestroy()
        {
            Core.GameServiceLocator.Unregister<MetaShopService>();
        }

        public void AddGold(int amount)
        {
            _data.Gold += amount;
            Save();
        }

        public void Purchase(string upgradeId)
        {
            MetaUpgradeState? state = _data.Upgrades.Find(u => u.Id == upgradeId);
            if (state == null)
            {
                state = new MetaUpgradeState { Id = upgradeId, Level = 0 };
                _data.Upgrades.Add(state);
            }

            state.Level += 1;
            Save();
        }

        public void SetDailySeed(string seed)
        {
            _data.LastDailySeed = seed;
            Save();
        }

        private void Load()
        {
            string path = GetPath();
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                _data = JsonUtility.FromJson<MetaShopSaveData>(json) ?? new MetaShopSaveData();
            }
        }

        private void Save()
        {
            string path = GetPath();
            string json = JsonUtility.ToJson(_data, true);
            File.WriteAllText(path, json);
        }

        private string GetPath()
        {
            return Path.Combine(Application.persistentDataPath, fileName);
        }
    }
}
