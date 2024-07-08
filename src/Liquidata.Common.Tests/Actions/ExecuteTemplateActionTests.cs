using Liquidata.Common.Actions;
using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Exceptions;
using Liquidata.Common.Services.Interfaces;
using Moq;

namespace Liquidata.Common.Tests.Actions
{
    public class ExecuteTemplateActionTests
    {
        [Fact]
        public async Task GivenCall_WhenInvalid_ThenErrors()
        {
            await Task.Yield();
            var action = new ExecuteTemplateAction();
            action.ExecutionTemplateId = null;

            var errors = action.BuildValidationErrors();
            Assert.NotEmpty(errors);
        }

        [Fact]
        public async Task GivenCall_WhenValid_ThenErrors()
        {
            await Task.Yield();
            var action = new ExecuteTemplateAction();
            action.ExecutionTemplateId = Guid.NewGuid();

            var errors = action.BuildValidationErrors();
            Assert.Empty(errors);
        }

        [Fact]
        public async Task GivenCall_WhenDisabled_ThenNoAction()
        {
            var executionService = new Mock<IExecutionService>();

            var action = new ExecuteTemplateAction { IsDisabled = true };
            var returnType = await action.ExecuteActionAsync(executionService.Object);

            Assert.Equal(ExecutionReturnType.Continue, returnType);
        }

        [Fact]
        public async Task GivenCall_WhenNoTemplateId_ThenException()
        {
            var executionService = new Mock<IExecutionService>();
            executionService.Setup(x => x.Project).Returns(new Project());

            var action = new ExecuteTemplateAction { ExecutionTemplateId = null };
            var exception = await Assert.ThrowsAsync<ExecutionException>(async () => await action.ExecuteActionAsync(executionService.Object));

            Assert.Contains("Unable to find", exception.Message);
        }

        [Fact]
        public async Task GivenCall_WhenTemplate_ThenExecuted()
        {
            var project = new Project();
            var template = new Mock<Template>();                        

            project.AllTemplates.Add(template.Object);
            
            var executionService = new Mock<IExecutionService>();
            executionService.Setup(x => x.Project).Returns(project);

            var action = new ExecuteTemplateAction { ExecutionTemplateId = template.Object.ActionId };
            var returnType = await action.ExecuteActionAsync(executionService.Object);

            Assert.Equal(ExecutionReturnType.Continue, returnType);
            template.Verify(x => x.ExecuteActionAsync(It.IsAny<IExecutionService>()), Times.Once());
        }
    }
}