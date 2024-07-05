using Liquidata.Common.Actions;
using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Services.Interfaces;
using Moq;

namespace Liquidata.Common.Tests.Actions
{
    public class ReloadActionTests
    {
        [Fact]
        public async Task GivenCall_WhenValidation_ThenNoErrors()
        {
            await Task.Yield();
            var action = new ReloadAction();
            var errors = action.BuildValidationErrors();

            Assert.Empty(errors);
        }

        [Fact]
        public async Task GivenCall_WhenDisabled_ThenNoAction()
        {
            var executionService = new Mock<IExecutionService>();

            var action = new ReloadAction { IsDisabled = true };
            var returnType = await action.ExecuteActionAsync(executionService.Object);

            Assert.Equal(ExecutionReturnType.Continue, returnType);
        }

        [Fact]
        public async Task GivenCall_WhenExecuted_ThenReloaded()
        {
            var browser = new Mock<IBrowserService>();
            var executionService = new Mock<IExecutionService>();            
            executionService.Setup(x => x.Browser).Returns(browser.Object);

            var action = new ReloadAction();
            var returnType = await action.ExecuteActionAsync(executionService.Object);

            Assert.Equal(ExecutionReturnType.Continue, returnType);
            executionService.Verify(x => x.Browser.ReloadPageAsync(), Times.Once());
        }
    }
}