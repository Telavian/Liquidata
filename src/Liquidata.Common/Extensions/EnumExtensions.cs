using System.Text;

namespace Liquidata.Common.Extensions
{
    public static class EnumExtensions
    {
        public static string BuildFriendlyName(this Enum item)
        {
            var text = item
                .ToString();

            var result = new StringBuilder(text.Length + 1);

            for (var x = 0; x < text.Length; x++)
            {
                var current = text[x];
                if (x > 0 && char.IsUpper(current))
                {
                    result.Append(' ');
                }

                result.Append(current);
            }

            return result.ToString();
        }
    }
}
