#nullable enable
using System;
using System.Collections.Generic;

namespace VHDPV2.Utils
{
    public static class WeightedRandomPicker
    {
        public static T Pick<T>(IReadOnlyList<T> items, IReadOnlyList<float> weights, float roll)
        {
            if (items.Count != weights.Count)
            {
                throw new ArgumentException("Item count must match weights count");
            }

            float total = 0f;
            for (int i = 0; i < weights.Count; i++)
            {
                total += weights[i];
            }

            if (total <= 0f)
            {
                throw new ArgumentException("Total weight must be positive");
            }

            float threshold = roll * total;
            float cumulative = 0f;
            for (int i = 0; i < items.Count; i++)
            {
                cumulative += weights[i];
                if (threshold <= cumulative)
                {
                    return items[i];
                }
            }

            return items[^1];
        }
    }
}
