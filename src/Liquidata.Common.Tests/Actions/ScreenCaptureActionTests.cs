using Liquidata.Common.Actions;
using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Exceptions;
using Liquidata.Common.Services.Interfaces;
using Moq;

namespace Liquidata.Common.Tests.Actions;

public class ScreenCaptureActionTests
{
    [Fact]
    public async Task GivenCall_WhenValid_ThenNoErrors()
    {
        await Task.Yield();
        var action = new ScreenCaptureAction { Name = "xyz" };
        var errors = action.BuildValidationErrors();

        Assert.Empty(errors);
    }

    [Fact]
    public async Task GivenCall_WhenNoName_ThenErrors()
    {
        await Task.Yield();
        var action = new ScreenCaptureAction { Name = "" };
        var errors = action.BuildValidationErrors();

        Assert.NotEmpty(errors);
    }

    [Fact]
    public async Task GivenCall_WhenDisabled_ThenNoAction()
    {
        var executionService = new Mock<IExecutionService>();

        var action = new ScreenCaptureAction { IsDisabled = true };
        var returnType = await action.ExecuteActionAsync(executionService.Object);

        Assert.Equal(ExecutionReturnType.Continue, returnType);
    }

    [Fact]
    public async Task GivenCall_WhenNoName_ThenException()
    {
        var executionService = new Mock<IExecutionService>();

        var action = new ScreenCaptureAction { Name = "" };
        var error = await Assert.ThrowsAsync<ExecutionException>(async () => await action.ExecuteActionAsync(executionService.Object));;

        Assert.Contains("not defined", error.Message);
    }

    [Fact]
    public async Task GivenCall_WhenExecuted_ThenScreenshotAdded()
    {
        var browser = new Mock<IBrowserService>();
        browser.Setup(x => x.GetScreenshotAsync())
            .ReturnsAsync([1, 2, 3]);

        var dataHandler = new Mock<IDataHandlerService>();
        dataHandler.Setup(x => x.AddScreenshotAsync(It.IsAny<string>(), It.IsAny<byte[]>()));

        var executionService = new Mock<IExecutionService>();
        executionService.Setup(x => x.Browser).Returns(browser.Object);
        executionService.Setup(x => x.DataHandler).Returns(dataHandler.Object);

        var action = new ScreenCaptureAction { Name = "xyz" };
        var returnType = await action.ExecuteActionAsync(executionService.Object);

        Assert.Equal(ExecutionReturnType.Continue, returnType);
        browser.Verify(x => x.GetScreenshotAsync(), Times.Once);
        dataHandler.Verify(x => x.AddScreenshotAsync(It.IsAny<string>(), It.IsAny<byte[]>()), Times.Once);
    }
}