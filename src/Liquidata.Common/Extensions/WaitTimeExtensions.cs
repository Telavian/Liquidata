using Liquidata.Common.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liquidata.Common.Extensions
{
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
}
