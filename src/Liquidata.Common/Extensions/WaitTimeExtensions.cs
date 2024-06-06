using System.Text;

namespace Liquidata.Common.Extensions;

public static class WaitActionExtensions
{
    public static string BuildWaitTimeText(this int milliseconds)
    {
        if (milliseconds == 0) 
        {
            return "No wait";
        }

        var time = TimeSpan.FromMilliseconds(milliseconds);

        var result = new StringBuilder();
        result.Append("Wait ");

        if (time.Seconds > 0)
        {
            result.Append(time.Seconds.ToString());
            result.Append(" s");

            if (time.Milliseconds > 0)
            {
                result.Append(", ");
            }
        }

        if (time.Milliseconds > 0)
        {
            result.Append(time.Milliseconds.ToString());
            result.Append(" ms");
        }

        return result
            .ToString();
    }
}
