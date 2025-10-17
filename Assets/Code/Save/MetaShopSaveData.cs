#nullable enable
using System;
using System.Collections.Generic;

namespace VHDPV2.Save
{
    [Serializable]
    public sealed class MetaShopSaveData
    {
        public int Gold;
        public List<MetaUpgradeState> Upgrades = new();
        public string LastDailySeed = string.Empty;
        public string Version = string.Empty;
    }

    [Serializable]
    public sealed class MetaUpgradeState
    {
        public string Id = string.Empty;
        public int Level;
    }
}
