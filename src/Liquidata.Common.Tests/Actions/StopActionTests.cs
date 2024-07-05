using Liquidata.Common.Actions;
using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Services.Interfaces;
using Moq;

namespace Liquidata.Common.Tests.Actions
{
    public class StopActionTests
    {
        [Fact]
        public async Task GivenCall_WhenValidation_ThenNoErrors()
        {
            await Task.Yield();
            var action = new StopAction();
            var errors = action.BuildValidationErrors();

            Assert.Empty(errors);
        }

        [Fact]
        public async Task GivenCall_WhenDisabled_ThenNoAction()
        {
            var executionService = new Mock<IExecutionService>();

            var action = new StopAction { IsDisabled = true };
            var returnType = await action.ExecuteActionAsync(executionService.Object);

            Assert.Equal(ExecutionReturnType.Continue, returnType);
        }

        [Theory]
        [InlineData(StopType.Loop, ExecutionReturnType.StopLoop)]
        [InlineData(StopType.Template, ExecutionReturnType.StopTemplate)]
        [InlineData(StopType.Project, ExecutionReturnType.StopProject)]
        public async Task GivenCall_WhenExecuted_ThenCorrectTypeReturned(StopType stopType, ExecutionReturnType returnType)
        {
            var executionService = new Mock<IExecutionService>();            

            var action = new StopAction { StopType = stopType };
            var actualReturnType = await action.ExecuteActionAsync(executionService.Object);

            Assert.Equal(returnType, actualReturnType);            
        }
    }
}