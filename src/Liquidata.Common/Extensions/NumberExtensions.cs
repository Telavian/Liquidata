namespace Liquidata.Common.Extensions
{
    public static class NumberExtensions
    {
        public static float RoundUpToNearest(this float number, float round)
        {
            return (float)((double)number).RoundUpToNearest(round);
        }

        public static double RoundUpToNearest(this double number, double round)
        {
            return round == 0 
                ? number
                : Math.Ceiling(number / round) * round;
        }

        public static float RoundDownToNearest(this float number, float round)
        {
            return (float)((double)number).RoundDownToNearest(round);
        }

        public static double RoundDownToNearest(this double number, double round)
        {
            return round == 0 
                ? number
                : Math.Floor(number / round) * round;
        }
    }
}
