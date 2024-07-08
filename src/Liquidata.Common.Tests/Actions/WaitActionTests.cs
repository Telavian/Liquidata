using Liquidata.Common.Actions;
using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Services.Interfaces;
using Moq;
using System.Diagnostics;

namespace Liquidata.Common.Tests.Actions;

public class WaitActionTests
{
    [Fact]
    public async Task GivenCall_WhenValidation_ThenNoErrors()
    {
        await Task.Yield();
        var action = new WaitAction();
        var errors = action.BuildValidationErrors();

        Assert.Empty(errors);
    }

    [Fact]
    public async Task GivenCall_WhenDisabled_ThenNoAction()
    {
        var executionService = new Mock<IExecutionService>();

        var action = new WaitAction { IsDisabled = true };
        var returnType = await action.ExecuteActionAsync(executionService.Object);

        Assert.Equal(ExecutionReturnType.Continue, returnType);
    }

    [Fact]
    public async Task GivenCall_WhenExecuted_ThenWait()
    {
        var waitms = 1000;
        var executionService = new Mock<IExecutionService>();

        var timer = Stopwatch.StartNew();
        var action = new WaitAction { WaitMilliseconds = waitms };
        var returnType = await action.ExecuteActionAsync(executionService.Object);

        Assert.Equal(ExecutionReturnType.Continue, returnType);
        Assert.True(timer.ElapsedMilliseconds >= waitms);
    }
}