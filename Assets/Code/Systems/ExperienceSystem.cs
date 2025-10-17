#nullable enable
using System;
using UnityEngine;

namespace VHDPV2.Systems
{
    public sealed class ExperienceSystem : MonoBehaviour
    {
        [SerializeField] private AnimationCurve levelCurve = new(new Keyframe(1, 0), new Keyframe(100, 10000));

        public event Action<int>? LevelChanged;
        public event Action<float, float>? ExperienceChanged;

        public int CurrentLevel { get; private set; } = 1;
        public float CurrentExperience { get; private set; }
        public float ExperienceToNext => GetExperienceRequirement(CurrentLevel + 1);

        public void GainExperience(float amount)
        {
            float required = ExperienceToNext;
            CurrentExperience += amount;
            while (CurrentExperience >= required)
            {
                CurrentExperience -= required;
                CurrentLevel += 1;
                LevelChanged?.Invoke(CurrentLevel);
                required = GetExperienceRequirement(CurrentLevel + 1);
            }

            ExperienceChanged?.Invoke(CurrentExperience, required);
        }

        private float GetExperienceRequirement(int level)
        {
            return Mathf.Max(1f, levelCurve.Evaluate(level));
        }
    }
}
