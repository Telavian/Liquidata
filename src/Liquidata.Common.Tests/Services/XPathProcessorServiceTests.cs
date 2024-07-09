using Liquidata.Client.Services;
using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Services.Interfaces;
using Moq;

namespace Liquidata.Common.Tests.Services
{
    public class XPathProcessorServiceTests
    {
        [Fact]
        public async Task GivenXPathProcessor_WhenReplace_ThenXPathReplaced()
        {
            var xpath1 = "/h1[@class]/h2";
            var xpath2 = "/h2[@class]/h3";

            var browser = new Mock<IBrowserService>();
            browser.Setup(x => x.GetAllMatchesAsync(xpath1, It.IsAny<int>())).ReturnsAsync([xpath1]);
            browser.Setup(x => x.GetAllMatchesAsync(xpath2, It.IsAny<int>())).ReturnsAsync([xpath2]);

            var processor = new XPathProcessorService(browser.Object);

            var result = await processor.ProcessXPathOperationAsync(xpath1, xpath2, SelectionOperation.Replace);
            Assert.Equal(xpath2, result);
        }

        [Fact]
        public async Task GivenXPathProcessor_WhenCombine_ThenXPathCombined()
        {
            var xpath1 = "/h1[@class]/h2";
            var xpath2 = "/h2[@class]/h3";

            var browser = new Mock<IBrowserService>();
            browser.Setup(x => x.GetAllMatchesAsync(xpath1, It.IsAny<int>())).ReturnsAsync([xpath1]);
            browser.Setup(x => x.GetAllMatchesAsync(xpath2, It.IsAny<int>())).ReturnsAsync([xpath2]);

            var processor = new XPathProcessorService(browser.Object);

            var result = await processor.ProcessXPathOperationAsync(xpath1, xpath2, SelectionOperation.Combine);
            var expected = $"{xpath1} | {xpath2}";
            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task GivenXPathProcessor_WhenSimilar_ThenXPathMadeSimilar()
        {
            var xpath1 = "/h1[@class]/h2";
            var xpath2 = "/h2[@class]/h3";

            var browser = new Mock<IBrowserService>();
            browser.Setup(x => x.GetAllMatchesAsync(xpath1, It.IsAny<int>())).ReturnsAsync([xpath1]);
            browser.Setup(x => x.GetAllMatchesAsync(xpath2, It.IsAny<int>())).ReturnsAsync([xpath2]);

            var processor = new XPathProcessorService(browser.Object);

            var result = await processor.ProcessXPathOperationAsync(xpath1, xpath2, SelectionOperation.Similar);
            var expected = $"{xpath1.Replace("/", "//")} | {xpath2.Replace("/", "//")}";
            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task GivenXPathProcessor_WhenRemoved_ThenXPathRemoved()
        {
            var browser = new Mock<IBrowserService>();
            var processor = new XPathProcessorService(browser.Object);

            var xpath1 = "/h1[@class]/h2";
            var xpath2 = "/h2[@class]/h3";

            var result = await processor.ProcessXPathOperationAsync($"{xpath1} | {xpath2}", xpath1, SelectionOperation.Remove);
            Assert.Equal(xpath2, result);
        }

        [Theory]
        [InlineData("/h1[@class]/h2", null, "")]
        [InlineData(null, "/h1[@class]/h2", "/h1[@class]/h2")]
        [InlineData("/h1[@class]/h2", "/h2[@class]/h3", "(/h1[@class]/h2)/h2[@class]/h3")]
        public void GivenXPathProcessor_WhenMakeRelative_ThenValueReturned(string parent, string xpath, string expected)
        {
            var browser = new Mock<IBrowserService>();
            browser.Setup(x => x.GetAllMatchesAsync(parent, It.IsAny<int>())).ReturnsAsync([parent]);
            browser.Setup(x => x.GetAllMatchesAsync(xpath, It.IsAny<int>())).ReturnsAsync([xpath]);

            var processor = new XPathProcessorService(browser.Object);

            var result = processor.MakeRelativeXPathQuery(parent, xpath);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("/h1[@class]/h2", null, "/../..")]
        [InlineData("/h1[@class]/h2", "/h1[@class]/h2/h3", "/h3")]
        [InlineData("/h1[@class]/h2/h1", "/h1[@class]/h2/h3", "/../h3")]
        public async Task GivenXPathProcessor_WhenDetermineRelative_ThenRelativeReturned(string parent, string xpath, string expected)
        {
            var browser = new Mock<IBrowserService>();
            browser.Setup(x => x.GetAllMatchesAsync(parent, It.IsAny<int>())).ReturnsAsync([parent]);
            browser.Setup(x => x.GetAllMatchesAsync(xpath, It.IsAny<int>())).ReturnsAsync([xpath]);

            var processor = new XPathProcessorService(browser.Object);

            var result = await processor.DetermineRelativeXPathAsync(parent, xpath);
            Assert.Equal(expected, result);
        }
    }
}
