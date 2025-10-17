#nullable enable
namespace VHDPV2.Utils
{
    public static class CombatMath
    {
        public static float CalculateArmorReduction(float armor)
        {
            if (armor <= 0f)
            {
                return 0f;
            }

            armor = armor > 200f ? 200f : armor;
            return armor / (100f + armor);
        }
    }
}
