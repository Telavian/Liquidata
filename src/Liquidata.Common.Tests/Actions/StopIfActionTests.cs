using Liquidata.Common.Actions;
using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Exceptions;
using Liquidata.Common.Services.Interfaces;
using Moq;

namespace Liquidata.Common.Tests.Actions;

public class StopIfActionTests
{
    [Fact]
    public async Task GivenCall_WhenValid_ThenNoErrors()
    {
        await Task.Yield();
        var action = new StopIfAction { Script = "xyz" };
        var errors = action.BuildValidationErrors();

        Assert.Empty(errors);
    }

    [Fact]
    public async Task GivenCall_WhenNoScript_ThenErrors()
    {
        await Task.Yield();
        var action = new StopIfAction { Script = "" };
        var errors = action.BuildValidationErrors();

        Assert.NotEmpty(errors);
    }

    [Fact]
    public async Task GivenCall_WhenDisabled_ThenNoAction()
    {
        var executionService = new Mock<IExecutionService>();

        var action = new StopIfAction { IsDisabled = true };
        var returnType = await action.ExecuteActionAsync(executionService.Object);

        Assert.Equal(ExecutionReturnType.Continue, returnType);
    }

    [Fact]
    public async Task GivenCall_WhenNoScript_ThenException()
    {
        var executionService = new Mock<IExecutionService>();

        var action = new StopIfAction { Script = "" };
        var error = await Assert.ThrowsAsync<ExecutionException>(async () => await action.ExecuteActionAsync(executionService.Object));

        Assert.Contains("not defined", error.Message);
    }

    [Theory]
    [InlineData(StopType.Loop, ExecutionReturnType.StopLoop)]
    [InlineData(StopType.Template, ExecutionReturnType.StopTemplate)]
    [InlineData(StopType.Project, ExecutionReturnType.StopProject)]
    public async Task GivenCall_WhenExecuted_ThenCorrectTypeReturned(StopType stopType, ExecutionReturnType returnType)
    {
        var browser = new Mock<IBrowserService>();
        browser.Setup(x => x.ExecuteJavascriptAsync<bool>(It.IsAny<string>()))
            .ReturnsAsync((true, true));

        var executionService = new Mock<IExecutionService>();            
        executionService.Setup(x => x.Browser).Returns(browser.Object);

        var action = new StopIfAction { StopType = stopType, Script = "xyz" };
        var actualReturnType = await action.ExecuteActionAsync(executionService.Object);

        Assert.Equal(returnType, actualReturnType);            
    }
}