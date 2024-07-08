using Liquidata.Common.Actions;
using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Exceptions;
using Liquidata.Common.Services.Interfaces;
using Moq;

namespace Liquidata.Common.Tests.Actions
{
    public class SelectActionTests
    {
        [Fact]
        public async Task GivenCall_WhenValidation_ThenNoErrors()
        {
            await Task.Yield();
            var action = new SelectAction { Name = "xyz", XPath = "xyz" };
            var errors = action.BuildValidationErrors();

            Assert.Empty(errors);
        }

        [Fact]
        public async Task GivenCall_WhenNameInvalid_ThenErrors()
        {
            await Task.Yield();
            var action = new SelectAction { Name = "", XPath = "xyz" };
            var errors = action.BuildValidationErrors();

            Assert.NotEmpty(errors);
        }

        [Fact]
        public async Task GivenCall_WhenXPathInvalid_ThenErrors()
        {
            await Task.Yield();
            var action = new SelectAction { Name = "xyz", XPath = "" };
            var errors = action.BuildValidationErrors();

            Assert.NotEmpty(errors);
        }

        [Fact]
        public async Task GivenCall_WhenDisabled_ThenNoAction()
        {
            var executionService = new Mock<IExecutionService>();

            var action = new SelectAction { IsDisabled = true };
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

            var action = new SelectAction();
            var error = await Assert.ThrowsAsync<ExecutionException>(async () => await action.ExecuteActionAsync(executionService.Object));

            Assert.Contains("not defined", error.Message);
        }

        [Fact]
        public async Task GivenCall_WhenMatches_ThenExecuted()
        {
            var logMessage = Guid.NewGuid().ToString();
            var project = new Project();
            var xpathMatches = new string[] { "1", "2", "3" };

            var browser = new Mock<IBrowserService>();
            browser.Setup(x => x.GetAllMatchesAsync(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(xpathMatches);

            var executionService = new Mock<IExecutionService>();
            executionService.Setup(x => x.Browser).Returns(browser.Object);

            var action = new SelectAction { XPath = "xyz" };
            var logAction = (LogAction)action.AddChildAction(project, ActionType.Log);
            logAction.Script = logMessage;
            logAction.ExpressionType = ExpressionType.Text;

            var returnType = await action.ExecuteActionAsync(executionService.Object);

            Assert.Equal(ExecutionReturnType.Continue, returnType);
            executionService.Verify(x => x.LogMessageAsync(logMessage), Times.Exactly(xpathMatches.Length));
        }
    }
}