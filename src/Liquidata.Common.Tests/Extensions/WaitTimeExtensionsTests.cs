using Liquidata.Common.Extensions;

namespace Liquidata.Common.Tests.Extensions
{
    public class WaitTimeExtensionsTests
    {
        [Theory]
        [InlineData(-1, "No wait")]
        [InlineData(0, "No wait")]
        [InlineData(100, "Wait 100 ms")]
        [InlineData(1000, "Wait 1 s")]
        [InlineData(1234, "Wait 1 s, 234 ms")]
        public void GivenWaitTime_WhenConverted_ThenValid(int milliseconds, string expected)
        {
            var text = milliseconds.BuildWaitTimeText();
            Assert.Equal(expected, text);
        }
    }
}
