using Liquidata.Common.Actions;
using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Services.Interfaces;
using Moq;

namespace Liquidata.Common.Tests.Actions;

public class ScrollActionTests
{
    [Fact]
    public async Task GivenCall_WhenValidation_ThenNoErrors()
    {
        await Task.Yield();
        var action = new ScrollAction();
        var errors = action.BuildValidationErrors();

        Assert.Empty(errors);
    }

    [Fact]
    public async Task GivenCall_WhenDisabled_ThenNoAction()
    {
        var executionService = new Mock<IExecutionService>();

        var action = new ScrollAction { IsDisabled = true };
        var returnType = await action.ExecuteActionAsync(executionService.Object);

        Assert.Equal(ExecutionReturnType.Continue, returnType);
    }

    [Fact]
    public async Task GivenCall_WhenExecuted_ThenScrolled()
    {
        var browser = new Mock<IBrowserService>();
        
        var executionService = new Mock<IExecutionService>();
        executionService.Setup(x => x.Browser).Returns(browser.Object);

        var action = new ScrollAction();
        var returnType = await action.ExecuteActionAsync(executionService.Object);

        Assert.Equal(ExecutionReturnType.Continue, returnType);
        browser.Verify(x => x.ScrollPageAsync(It.IsAny<ScrollType>()), Times.Once());
    }
}