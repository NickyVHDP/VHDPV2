#nullable enable
using NUnit.Framework;
using VHDPV2.Utils;

namespace VHDPV2.Tests
{
    public sealed class CombatMathTests
    {
        [Test]
        public void ArmorReductionClamped()
        {
            float reduction = CombatMath.CalculateArmorReduction(500f);
            Assert.That(reduction, Is.LessThanOrEqualTo(0.667f));
        }
    }
}
