using Liquidata.Common.Actions;
using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Exceptions;
using Liquidata.Common.Services.Interfaces;
using Moq;

namespace Liquidata.Common.Tests.Actions
{
    public class RelativeSelectActionTests
    {
        [Fact]
        public async Task GivenCall_WhenValidation_ThenNoErrors()
        {
            await Task.Yield();
            var action = new RelativeSelectAction { Name = "xyz", XPath = "xyz" };
            var errors = action.BuildValidationErrors();

            Assert.Empty(errors);
        }

        [Fact]
        public async Task GivenCall_WhenNameInvalid_ThenErrors()
        {
            await Task.Yield();
            var action = new RelativeSelectAction { Name = "", XPath = "xyz" };
            var errors = action.BuildValidationErrors();

            Assert.NotEmpty(errors);
        }

        [Fact]
        public async Task GivenCall_WhenXPathInvalid_ThenErrors()
        {
            await Task.Yield();
            var action = new RelativeSelectAction { Name = "xyz", XPath = "" };
            var errors = action.BuildValidationErrors();

            Assert.NotEmpty(errors);
        }

        [Fact]
        public async Task GivenCall_WhenDisabled_ThenNoAction()
        {
            var executionService = new Mock<IExecutionService>();

            var action = new RelativeSelectAction { IsDisabled = true };
            var returnType = await action.ExecuteActionAsync(executionService.Object);

            Assert.Equal(ExecutionReturnType.Continue, returnType);
        }

        [Fact]
        public async Task GivenCall_WhenNoSelection_ThenException()
        {
            var xpathProcessor = new Mock<IXPathProcessorService>();
            xpathProcessor.Setup(x => x.MakeRelativeXPathQuery(It.IsAny<string>(), It.IsAny<string>()))
                .Returns("");

            var executionService = new Mock<IExecutionService>();
            executionService.Setup(x => x.XPathProcessor).Returns(xpathProcessor.Object);

            var action = new RelativeSelectAction();
            var error = await Assert.ThrowsAsync<ExecutionException>(async () => await action.ExecuteActionAsync(executionService.Object));

            Assert.Contains("not defined", error.Message);
        }

        [Fact]
        public async Task GivenCall_WhenMatches_ThenExecuted()
        {
            var xpathProcessor = new Mock<IXPathProcessorService>();
            xpathProcessor.Setup(x => x.MakeRelativeXPathQuery(It.IsAny<string>(), It.IsAny<string>()))
                .Returns("xyz");

            var browser = new Mock<IBrowserService>();
            browser.Setup(x => x.GetAllMatchesAsync(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(new string[] { "1", "2", "3" });

            var executionService = new Mock<IExecutionService>();
            executionService.Setup(x => x.XPathProcessor).Returns(xpathProcessor.Object);
            executionService.Setup(x => x.Browser).Returns(browser.Object);

            var action = new RelativeSelectAction();
            var returnType = await action.ExecuteActionAsync(executionService.Object);

            Assert.Equal(ExecutionReturnType.Continue, returnType);
        }
    }
}