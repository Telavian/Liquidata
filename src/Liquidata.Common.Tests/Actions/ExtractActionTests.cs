using Liquidata.Common.Actions;
using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Exceptions;
using Liquidata.Common.Services.Interfaces;
using Moq;

namespace Liquidata.Common.Tests.Actions
{
    public class ExtractActionTests
    {
        [Fact]
        public async Task GivenCall_WhenValid_ThenNoErrors()
        {
            await Task.Yield();
            var action = new ExtractAction();
            action.Name = "xyz";
            action.Script = "abc";
            var errors = action.BuildValidationErrors();

            Assert.Empty(errors);
        }

        [Fact]
        public async Task GivenCall_WhenInvalid_ThenErrors()
        {
            await Task.Yield();
            var action = new ExtractAction();            
            var errors = action.BuildValidationErrors();

            Assert.NotEmpty(errors);
        }

        [Fact]
        public async Task GivenCall_WhenDisabled_ThenNoAction()
        {
            var executionService = new Mock<IExecutionService>();

            var action = new ExtractAction { IsDisabled = true };
            var returnType = await action.ExecuteActionAsync(executionService.Object);

            Assert.Equal(ExecutionReturnType.Continue, returnType);
        }

        [Fact]
        public async Task GivenCall_WhenNoScript_ThenException()
        {
            var executionService = new Mock<IExecutionService>();

            var action = new ExtractAction { Script = null };

            var error = await Assert.ThrowsAsync<ExecutionException>(async () => await action.ExecuteActionAsync(executionService.Object));
            Assert.Contains("not defined", error.Message);
        }

        [Fact]
        public async Task GivenCall_WhenNoName_ThenException()
        {
            var executionService = new Mock<IExecutionService>();

            var action = new ExtractAction { Script = "xyz", Name = null! };

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

            var action = new ExtractAction { Name = "name", Script = "script" };
            var error = await Assert.ThrowsAsync<ExecutionException>(async () => await action.ExecuteActionAsync(executionService.Object));

            Assert.Contains("not executed", error.Message);
        }

        [Fact]
        public async Task GivenCall_WhenExecuted_ThenExtracted()
        {
            var dataResult = "data";

            var browser = new Mock<IBrowserService>();
            browser.Setup(x => x.ExecuteJavascriptAsync<string>(It.IsAny<string>()))
                .ReturnsAsync((true, dataResult));

            var dataHandler = new Mock<IDataHandlerService>();
            dataHandler.Setup(x => x.CleanDataAsync(It.IsAny<string>(), It.IsAny<FieldType>()))
                .ReturnsAsync(dataResult);

            var executionService = new Mock<IExecutionService>();            
            executionService.Setup(x => x.Browser).Returns(browser.Object);
            executionService.Setup(x => x.DataHandler).Returns(dataHandler.Object);

            var action = new ExtractAction { Name = "name", Script = "script" };
            var returnType = await action.ExecuteActionAsync(executionService.Object);

            Assert.Equal(ExecutionReturnType.Continue, returnType);
            executionService.Verify(x => x.DataHandler.AddData(action.Name, dataResult), Times.Once());
        }
    }
}