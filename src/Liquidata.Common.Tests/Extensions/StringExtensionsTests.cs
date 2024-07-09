using Liquidata.Common.Extensions;

namespace Liquidata.Common.Tests.Extensions
{
    public class StringExtensionsTests
    {
        [Fact]
        public void GivenString_WhenStringJoin_ThenJoined()
        {
            var items = new[] { Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString() };
            var separator = "!";

            var expected = string.Join(separator, items);
            var actual = items.StringJoin(separator);

            Assert.Equal(expected, actual);
        }
    }
}
