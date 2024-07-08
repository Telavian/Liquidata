using Liquidata.Common.Actions;
using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Services.Interfaces;
using Moq;

namespace Liquidata.Common.Tests.Actions
{
    public class TemplateTests
    {
        [Fact]
        public async Task GivenCall_WhenValidation_ThenNoErrors()
        {
            await Task.Yield();
            var action = new Template();
            var errors = action.BuildValidationErrors();

            Assert.Empty(errors);
        }

        [Fact]
        public async Task GivenCall_WhenDisabled_ThenNoAction()
        {
            var executionService = new Mock<IExecutionService>();

            var action = new Template { IsDisabled = true };
            var returnType = await action.ExecuteActionAsync(executionService.Object);

            Assert.Equal(ExecutionReturnType.Continue, returnType);
        }

        [Fact]
        public async Task GivenCall_WhenExecuted_ThenChildrenExecuted()
        {
            var logMessage = Guid.NewGuid().ToString();
            var project = new Project();

            var executionService = new Mock<IExecutionService>();            

            var action = new Template();
            var logAction = (LogAction)action.AddChildAction(project, ActionType.Log);
            logAction.Script = logMessage;
            logAction.ExpressionType = ExpressionType.Text;

            var returnType = await action.ExecuteActionAsync(executionService.Object);

            Assert.Equal(ExecutionReturnType.Continue, returnType);
            executionService.Verify(x => x.LogMessageAsync(logMessage), Times.Once());
        }
    }
}