using Liquidata.Common.Actions;
using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Services.Interfaces;
using Moq;

namespace Liquidata.Common.Tests.Actions
{
    public class ScopeActionTests
    {
        [Fact]
        public async Task GivenCall_WhenValid_ThenNoErrors()
        {
            await Task.Yield();
            var action = new ScopeAction { Name = "xyz" };
            var errors = action.BuildValidationErrors();

            Assert.Empty(errors);
        }

        [Fact]
        public async Task GivenCall_WhenNoName_ThenErrors()
        {
            await Task.Yield();
            var action = new ScopeAction { Name = "" };
            var errors = action.BuildValidationErrors();

            Assert.NotEmpty(errors);
        }

        [Fact]
        public async Task GivenCall_WhenDisabled_ThenNoAction()
        {
            var executionService = new Mock<IExecutionService>();

            var action = new ScopeAction { IsDisabled = true };
            var returnType = await action.ExecuteActionAsync(executionService.Object);

            Assert.Equal(ExecutionReturnType.Continue, returnType);
        }

        [Fact]
        public async Task GivenCall_WhenExecuted_ThenScopeUpdated()
        {
            var logMessage = Guid.NewGuid().ToString();
            var project = new Project();
            var dataHandler = new Mock<IDataHandlerService>();

            var executionService = new Mock<IExecutionService>();
            executionService.Setup(x => x.DataHandler).Returns(dataHandler.Object);

            var action = new ScopeAction();
            var logAction = (LogAction)action.AddChildAction(project, ActionType.Log);
            logAction.Script = logMessage;
            logAction.ExpressionType = ExpressionType.Text;

            var returnType = await action.ExecuteActionAsync(executionService.Object);

            Assert.Equal(ExecutionReturnType.Continue, returnType);
            dataHandler.VerifySet(x => x.DataScope = It.IsAny<string>(), Times.Exactly(2));
            executionService.Verify(x => x.LogMessageAsync(logMessage), Times.Once());
        }
    }
}