#nullable enable
using NUnit.Framework;
using UnityEngine;
using VHDPV2.Systems;

namespace VHDPV2.Tests
{
    public sealed class ExperienceSystemTests
    {
        [Test]
        public void GainExperienceLevelsUpAtThreshold()
        {
            var gameObject = new GameObject("experience");
            var system = gameObject.AddComponent<ExperienceSystem>();
            system.GainExperience(system.ExperienceToNext);
            Assert.That(system.CurrentLevel, Is.EqualTo(2));
        }
    }
}
