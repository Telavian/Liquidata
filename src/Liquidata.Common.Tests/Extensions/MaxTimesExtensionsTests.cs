using Liquidata.Common.Extensions;

namespace Liquidata.Common.Tests.Extensions;

public class MaxTimesExtensionsTests
{
    [Theory]
    [InlineData(-1, "xyz", "xyz unlimited times")]
    [InlineData(0, "xyz", "xyz unlimited times")]
    [InlineData(1, "xyz", "xyz 1 time")]
    [InlineData(5, "xyz", "xyz 5 times")]
    public void GivenMaxTimes_WhenConverted_ThenValid(int maxTime, string prefix, string expected)
    {
        var text = maxTime.BuildMaxTimesText(prefix);
        Assert.Equal(expected, text);
    }
}
