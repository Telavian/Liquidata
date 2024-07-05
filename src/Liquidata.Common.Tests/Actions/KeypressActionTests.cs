using Liquidata.Common.Actions;
using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Exceptions;
using Liquidata.Common.Services.Interfaces;
using Moq;

namespace Liquidata.Common.Tests.Actions
{
    public class KeypressActionTests
    {
        [Fact]
        public async Task GivenCall_WhenValidation_ThenNoErrors()
        {
            await Task.Yield();
            var action = new KeypressAction { Keypressed = "xyz" };
            var errors = action.BuildValidationErrors();

            Assert.Empty(errors);
        }

        [Fact]
        public async Task GivenCall_WhenInvalid_ThenErrors()
        {
            await Task.Yield();
            var action = new KeypressAction { Keypressed = null! };
            var errors = action.BuildValidationErrors();

            Assert.NotEmpty(errors);
        }

        [Fact]
        public async Task GivenCall_WhenDisabled_ThenNoAction()
        {
            var executionService = new Mock<IExecutionService>();

            var action = new KeypressAction { IsDisabled = true };
            var returnType = await action.ExecuteActionAsync(executionService.Object);

            Assert.Equal(ExecutionReturnType.Continue, returnType);
        }

        [Fact]
        public async Task GivenCall_WhenNoKeypressed_ThenException()
        {
            var executionService = new Mock<IExecutionService>();

            var action = new KeypressAction { Keypressed = null! };
            var error = await Assert.ThrowsAsync<ExecutionException>(async () => await action.ExecuteActionAsync(executionService.Object));

            Assert.Contains("not defined", error.Message);
        }

        [Fact]
        public async Task GivenCall_WhenExecuted_ThenDataInput()
        {
            var browser = new Mock<IBrowserService>();
            
            var executionService = new Mock<IExecutionService>();            
            executionService.Setup(x => x.Browser).Returns(browser.Object);

            var action = new KeypressAction { Keypressed = "xyz" };
            var returnType = await action.ExecuteActionAsync(executionService.Object);

            Assert.Equal(ExecutionReturnType.Continue, returnType);
            browser.Verify(x => x.KeypressToSelectionAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), action.Keypressed), Times.Once());
        }
    }
}