using Liquidata.Common.Extensions;

namespace Liquidata.Common.Tests.Extensions
{
    public class ValidationExtensionsTests
    {
        [Theory]
        [InlineData(null, false)]
        [InlineData("", false)]
        [InlineData("abc", true)]
        public void GivenString_WhenIsDefined_ThenCorrect(string? text, bool expected)
        {
            var result = text.IsDefined();
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(null, true)]
        [InlineData("", true)]
        [InlineData("abc", false)]
        public void GivenString_WhenIsNotDefined_ThenCorrect(string? text, bool expected)
        {
            var result = text.IsNotDefined();
            Assert.Equal(expected, result);
        }

        [Fact]
        public void GivenGuid_WhenIsDefined_ThenCorrect()
        {
            var result = ((Guid?)null).IsDefined();
            Assert.False(result);

            result = ((Guid?)Guid.Empty).IsDefined();
            Assert.False(result);

            result = ((Guid?)Guid.NewGuid()).IsDefined();
            Assert.True(result);
        }

        [Fact]
        public void GivenGuid_WhenIsNotDefined_ThenCorrect()
        {
            var result = ((Guid?)null).IsNotDefined();
            Assert.True(result);

            result = ((Guid?)Guid.Empty).IsNotDefined();
            Assert.True(result);

            result = ((Guid?)Guid.NewGuid()).IsNotDefined();
            Assert.False(result);
        }
    }
}
