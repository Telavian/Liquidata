using Liquidata.Common.Actions;
using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Exceptions;
using Liquidata.Common.Services.Interfaces;
using Moq;

namespace Liquidata.Common.Tests.Actions;

public class InputActionTests
{
    [Fact]
    public async Task GivenCall_WhenValidation_ThenNoErrors()
    {
        await Task.Yield();
        var action = new InputAction { Script = "xyz" };
        var errors = action.BuildValidationErrors();

        Assert.Empty(errors);
    }

    [Fact]
    public async Task GivenCall_WhenInvalid_ThenErrors()
    {
        await Task.Yield();
        var action = new InputAction { Script = null };
        var errors = action.BuildValidationErrors();

        Assert.NotEmpty(errors);
    }

    [Fact]
    public async Task GivenCall_WhenDisabled_ThenNoAction()
    {
        var executionService = new Mock<IExecutionService>();

        var action = new InputAction { IsDisabled = true };
        var returnType = await action.ExecuteActionAsync(executionService.Object);

        Assert.Equal(ExecutionReturnType.Continue, returnType);
    }

    [Fact]
    public async Task GivenCall_WhenNoScript_ThenException()
    {
        var executionService = new Mock<IExecutionService>();

        var action = new InputAction { Script = null };
        var error = await Assert.ThrowsAsync<ExecutionException>(async () => await action.ExecuteActionAsync(executionService.Object));

        Assert.Contains("not defined", error.Message);
    }

    [Fact]
    public async Task GivenCall_WhenExecuted_ThenDataInput()
    {
        var browser = new Mock<IBrowserService>();
        
        var executionService = new Mock<IExecutionService>();            
        executionService.Setup(x => x.Browser).Returns(browser.Object);

        var action = new InputAction { Script = "xyz", ExpressionType = ExpressionType.Text };
        var returnType = await action.ExecuteActionAsync(executionService.Object);

        Assert.Equal(ExecutionReturnType.Continue, returnType);
        browser.Verify(x => x.InputToSelectionAsync(It.IsAny<string>(), action.Script), Times.Once());
    }
}