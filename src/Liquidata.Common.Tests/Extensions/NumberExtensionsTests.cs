using Liquidata.Common.Extensions;

namespace Liquidata.Common.Tests.Extensions
{
    public class NumberExtensionsTests
    {
        [Theory]
        [InlineData(100F, 125F, 125F)]
        [InlineData(200F, 125F, 250F)]
        [InlineData(250, 125F, 250F)]
        public void GivenFloat_WhenRoundUpToNearest_ThenRounded(float number, float round, float expected)
        {
            var result = number.RoundUpToNearest(round);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(100D, 125D, 125D)]
        [InlineData(200D, 125D, 250D)]
        [InlineData(250, 125D, 250D)]
        public void GivenDouble_WhenRoundUpToNearest_ThenRounded(double number, double round, double expected)
        {
            var result = number.RoundUpToNearest(round);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(900F, 125F, 875F)]
        [InlineData(755F, 125F, 750F)]
        [InlineData(1000F, 125F, 1000F)]
        public void GivenFloat_WhenRoundDownToNearest_ThenRounded(float number, float round, float expected)
        {
            var result = number.RoundDownToNearest(round);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(900D, 125D, 875D)]
        [InlineData(755D, 125D, 750D)]
        [InlineData(1000D, 125D, 1000D)]
        public void GivenDouble_WhenRoundDownToNearest_ThenRounded(double number, double round, double expected)
        {
            var result = number.RoundDownToNearest(round);
            Assert.Equal(expected, result);
        }
    }
}
