#nullable enable
using System.Collections.Generic;
using NUnit.Framework;
using VHDPV2.Utils;

namespace VHDPV2.Tests
{
    public sealed class WeightedRandomPickerTests
    {
        [Test]
        public void PickReturnsExpectedIndex()
        {
            var items = new List<string> { "Common", "Rare", "Legendary" };
            var weights = new List<float> { 0.6f, 0.3f, 0.1f };
            string result = WeightedRandomPicker.Pick(items, weights, 0.95f);
            Assert.That(result, Is.EqualTo("Legendary"));
        }
    }
}
