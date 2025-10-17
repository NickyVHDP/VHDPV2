#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;

namespace VHDPV2.Core
{
    public sealed class ObjectPoolService : MonoBehaviour
    {
        [Serializable]
        private sealed class PoolConfig
        {
            public GameObject? Prefab;
            public int InitialSize = 8;
        }

        [SerializeField] private List<PoolConfig> initialPools = new();
        private readonly Dictionary<GameObject, Queue<GameObject>> _pools = new();

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            GameServiceLocator.Register(this);
            foreach (PoolConfig config in initialPools)
            {
                if (config.Prefab == null)
                {
                    continue;
                }

                WarmPool(config.Prefab, config.InitialSize);
            }
        }

        private void OnDestroy()
        {
            GameServiceLocator.Unregister<ObjectPoolService>();
        }

        public void WarmPool(GameObject prefab, int count)
        {
            if (!_pools.TryGetValue(prefab, out Queue<GameObject>? queue))
            {
                queue = new Queue<GameObject>();
                _pools[prefab] = queue;
            }

            for (int i = queue.Count; i < count; i++)
            {
                GameObject instance = Instantiate(prefab);
                instance.SetActive(false);
                queue.Enqueue(instance);
            }
        }

        public GameObject Get(GameObject prefab)
        {
            if (!_pools.TryGetValue(prefab, out Queue<GameObject>? queue) || queue.Count == 0)
            {
                WarmPool(prefab, 1);
                queue = _pools[prefab];
            }

            GameObject instance = queue.Dequeue();
            instance.SetActive(true);
            return instance;
        }

        public void Release(GameObject prefab, GameObject instance)
        {
            if (!_pools.TryGetValue(prefab, out Queue<GameObject>? queue))
            {
                queue = new Queue<GameObject>();
                _pools[prefab] = queue;
            }

            instance.SetActive(false);
            queue.Enqueue(instance);
        }
    }
}
