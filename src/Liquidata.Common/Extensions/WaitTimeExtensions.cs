using Liquidata.Common.Actions;
using System.Text;

namespace Liquidata.Common.Extensions;

public static class WaitActionExtensions
{
    public static string BuildWaitTimeText(this WaitAction action)
    {
        return action.WaitMilliseconds
            .BuildWaitTimeText();
    }

    public static string BuildWaitTimeText(this int milliseconds)
    {
        var time = TimeSpan.FromMilliseconds(milliseconds);

        if (milliseconds == 0)
        {
            return "0 ms";
        }

        var result = new StringBuilder();

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
