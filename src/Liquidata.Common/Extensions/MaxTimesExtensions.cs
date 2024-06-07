namespace Liquidata.Common.Extensions
{
    public static class MaxTimesExtensions
    {
        public static string BuildMaxTimesText(this int maxTimes, string prefix = "")
        {
            if (maxTimes <= 0)
            {
                return $"{prefix} unlimited times"
                    .Trim();
            }

            if (maxTimes == 1)
            {
                return $"{prefix} 1 time"
                    .Trim();
            }

            return $"{prefix} {maxTimes} times"
                .Trim();
        }
    }
}
