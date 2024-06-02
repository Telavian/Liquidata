using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liquidata.Common.Extensions
{
    public static class StringExtensions
    {
        public static string StringJoin(this IEnumerable<string> items, string separator)
        {
            return string.Join(separator, items);
        }
    }
}
