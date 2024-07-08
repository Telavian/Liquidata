using Liquidata.Common.Actions;
using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Exceptions;
using Liquidata.Common.Services.Interfaces;
using Moq;

namespace Liquidata.Common.Tests.Actions;

public class ExecuteScriptActionTests
{
    [Fact]
    public async Task GivenCall_WhenInvalid_ThenErrors()
    {
        await Task.Yield();
        var action = new ExecuteScriptAction();
        action.Script = null;

        var errors = action.BuildValidationErrors();
        Assert.NotEmpty(errors);
    }

    [Fact]
    public async Task GivenCall_WhenDisabled_ThenNoAction()
    {
        var executionService = new Mock<IExecutionService>();

        var action = new ExecuteScriptAction { IsDisabled = true };
        var returnType = await action.ExecuteActionAsync(executionService.Object);

        Assert.Equal(ExecutionReturnType.Continue, returnType);
    }

    [Fact]
    public async Task GivenCall_WhenNoScript_ThenException()
    {
        var executionService = new Mock<IExecutionService>();

        var action = new ExecuteScriptAction { Script = null };
        var exception = await Assert.ThrowsAsync<ExecutionException>(async () => await action.ExecuteActionAsync(executionService.Object));

        Assert.Contains("Script is not defined", exception.Message);
    }

    [Fact]
    public async Task GivenCall_WhenScript_ThenExecuted()
    {
        var browser = new Mock<IBrowserService>();
        browser.Setup(x => x.ExecuteJavascriptAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        var executionService = new Mock<IExecutionService>();
        executionService.Setup(x => x.Browser).Returns(browser.Object);

        var action = new ExecuteScriptAction { Script = "xyz" };
        var returnType = await action.ExecuteActionAsync(executionService.Object);

        Assert.Equal(ExecutionReturnType.Continue, returnType);
        browser.Verify(x => x.ExecuteJavascriptAsync(action.Script), Times.Once());
    }

    [Fact]
    public async Task GivenCall_WhenScriptError_ThenException()
    {
        var browser = new Mock<IBrowserService>();
        browser.Setup(x => x.ExecuteJavascriptAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        var executionService = new Mock<IExecutionService>();
        executionService.Setup(x => x.Browser).Returns(browser.Object);

        var action = new ExecuteScriptAction { Script = "xyz" };

        var error = await Assert.ThrowsAsync<ExecutionException>(async () => await action.ExecuteActionAsync(executionService.Object));
        Assert.Contains("not executed", error.Message);
    }
}