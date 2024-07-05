using Liquidata.Common.Actions;
using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Services.Interfaces;
using Moq;

namespace Liquidata.Common.Tests.Actions
{
    public class ClickActionTests
    {
        [Fact]
        public async Task GivenCall_WhenInvalid_ThenErrors()
        {
            await Task.Yield();
            var action = new ClickAction();
            action.ClickType = ClickType.ExecuteTemplate;
            action.ExecutionTemplateId = null;

            var errors = action.BuildValidationErrors();

            Assert.NotEmpty(errors);
        }

        [Fact]
        public async Task GivenCall_WhenDisabled_ThenNoAction()
        {
            var executionService = new Mock<IExecutionService>();

            var action = new ClickAction { IsDisabled = true };
            var returnType = await action.ExecuteActionAsync(executionService.Object);

            Assert.Equal(ExecutionReturnType.Continue, returnType);
        }

        [Fact]
        public async Task GivenCall_WhenSelectionDisabled_ThenNoAction()
        {
            var browser = new Mock<IBrowserService>();
            browser.Setup(x => x.CheckSelectionDisabledAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            var executionService = new Mock<IExecutionService>();            
            executionService.Setup(x => x.Browser).Returns(browser.Object);

            var action = new ClickAction();
            var returnType = await action.ExecuteActionAsync(executionService.Object);

            Assert.Equal(ExecutionReturnType.Continue, returnType);
        }

        [Fact]
        public async Task GivenCall_WhenNewPage_ThenNewPage()
        {
            var browser = new Mock<IBrowserService>();
            browser.Setup(x => x.CheckSelectionDisabledAsync(It.IsAny<string>()))
                .ReturnsAsync(false);
            
            var template = new Mock<Template>();

            var project = new Project();
            project.AllTemplates.Add(template.Object);

            var executionService = new Mock<IExecutionService>();
            executionService.Setup(x => x.Browser).Returns(browser.Object);
            executionService.Setup(x => x.Project).Returns(project);

            var action = new ClickAction();
            action.IsNewPage = true;
            action.ExecutionTemplateId = template.Object.ActionId;

            var returnType = await action.ExecuteActionAsync(executionService.Object);

            Assert.Equal(ExecutionReturnType.Continue, returnType);
            executionService.Verify(x => x.CreateExecutionTaskAsync(It.IsAny<Func<Task>>()), Times.Once());
        }

        [Fact]
        public async Task GivenCall_WhenSamePage_ThenMouseClick()
        {
            var browser = new Mock<IBrowserService>();
            browser.Setup(x => x.CheckSelectionDisabledAsync(It.IsAny<string>()))
                .ReturnsAsync(false);
            browser.Setup(x => x.ClickSelectionAsync(It.IsAny<string>(), It.IsAny<ClickButton>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(Task.CompletedTask);

            var executionService = new Mock<IExecutionService>();
            executionService.Setup(x => x.Browser).Returns(browser.Object);

            var action = new ClickAction();
            var returnType = await action.ExecuteActionAsync(executionService.Object);

            Assert.Equal(ExecutionReturnType.Continue, returnType);
            browser.Verify(x => x.ClickSelectionAsync(It.IsAny<string>(), It.IsAny<ClickButton>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Once());
        }

        [Fact]
        public async Task GivenCall_WhenExecuteTemplate_ThenTemplateExecuted()
        {
            var browser = new Mock<IBrowserService>();
            browser.Setup(x => x.CheckSelectionDisabledAsync(It.IsAny<string>()))
                .ReturnsAsync(false);
            browser.Setup(x => x.ClickSelectionAsync(It.IsAny<string>(), It.IsAny<ClickButton>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(Task.CompletedTask);            

            var template = new Mock<Template>();

            var project = new Project();
            project.AllTemplates.Add(template.Object);

            var executionService = new Mock<IExecutionService>();
            executionService.Setup(x => x.Browser).Returns(browser.Object);
            executionService.Setup(x => x.Project).Returns(project);

            var action = new ClickAction();
            action.ClickType = ClickType.ExecuteTemplate;
            action.ExecutionTemplateId = template.Object.ActionId;

            var returnType = await action.ExecuteActionAsync(executionService.Object);

            Assert.Equal(ExecutionReturnType.Continue, returnType);
            template.Verify(x => x.ExecuteActionAsync(executionService.Object), Times.Once());
        }
    }
}