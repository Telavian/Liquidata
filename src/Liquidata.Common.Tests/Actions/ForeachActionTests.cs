using Liquidata.Common.Actions;
using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Exceptions;
using Liquidata.Common.Services.Interfaces;
using Moq;

namespace Liquidata.Common.Tests.Actions
{
    public class ForeachActionTests
    {
        [Fact]
        public async Task GivenCall_WhenValid_ThenNoErrors()
        {
            await Task.Yield();
            var action = new ForeachAction();
            action.Name = "xyz";
            action.Script = "abc";
            var errors = action.BuildValidationErrors();

            Assert.Empty(errors);
        }

        [Fact]
        public async Task GivenCall_WhenInvalid_ThenErrors()
        {
            await Task.Yield();
            var action = new ForeachAction();            
            var errors = action.BuildValidationErrors();

            Assert.NotEmpty(errors);
        }

        [Fact]
        public async Task GivenCall_WhenDisabled_ThenNoAction()
        {
            var executionService = new Mock<IExecutionService>();

            var action = new ForeachAction { IsDisabled = true };
            var returnType = await action.ExecuteActionAsync(executionService.Object);

            Assert.Equal(ExecutionReturnType.Continue, returnType);
        }

        [Fact]
        public async Task GivenCall_WhenNoScript_ThenException()
        {
            var executionService = new Mock<IExecutionService>();

            var action = new ForeachAction { Name = "xyz", Script = null };

            var error = await Assert.ThrowsAsync<ExecutionException>(async () => await action.ExecuteActionAsync(executionService.Object));
            Assert.Contains("not defined", error.Message);
        }

        [Fact]
        public async Task GivenCall_WhenNoName_ThenException()
        {
            var executionService = new Mock<IExecutionService>();

            var action = new ForeachAction { Script = "xyz", Name = null! };

            var error = await Assert.ThrowsAsync<ExecutionException>(async () => await action.ExecuteActionAsync(executionService.Object));
            Assert.Contains("not defined", error.Message);
        }

        [Fact]
        public async Task GivenCall_WhenExecutionError_ThenException()
        {
            var dataResult = "data";

            var browser = new Mock<IBrowserService>();
            browser.Setup(x => x.ExecuteJavascriptAsync<string>(It.IsAny<string>()))
                .ReturnsAsync((false, dataResult));

            var dataHandler = new Mock<IDataHandlerService>();

            var executionService = new Mock<IExecutionService>();
            executionService.Setup(x => x.Browser).Returns(browser.Object);
            executionService.Setup(x => x.DataHandler).Returns(dataHandler.Object);

            var action = new ForeachAction { Name = "name", Script = "script" };
            var error = await Assert.ThrowsAsync<ExecutionException>(async () => await action.ExecuteActionAsync(executionService.Object));

            Assert.Contains("not executed", error.Message);
        }

        [Fact]
        public async Task GivenCall_WhenExecuted_ThenExtracted()
        {
            var dataResult = new string[0];

            var browser = new Mock<IBrowserService>();
            browser.Setup(x => x.ExecuteJavascriptAsync<string[]>(It.IsAny<string>()))
                .ReturnsAsync((true, dataResult));

            var executionService = new Mock<IExecutionService>();
            executionService.Setup(x => x.Browser).Returns(browser.Object);

            var action = new ForeachAction { Name = "name", Script = "script" };
            var returnType = await action.ExecuteActionAsync(executionService.Object);

            Assert.Equal(ExecutionReturnType.Continue, returnType);
        }
    }
}