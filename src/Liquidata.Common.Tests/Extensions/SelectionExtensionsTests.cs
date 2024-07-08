using Liquidata.Common.Actions;
using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Extensions;

namespace Liquidata.Common.Tests.Extensions
{
    public class SelectionExtensionsTests
    {
        [Fact]
        public void GivenAction_WhenNoSelectionAncestors_ThenNoSelections()
        {
            var action = new LogAction();
            var ancestors = action.GetAllSelectionAncestors();

            Assert.Empty(ancestors);
        }

        [Fact]
        public void GivenSelectAction_WhenNoSelectionAncestors_ThenSelf()
        {
            var action = new SelectAction();
            var ancestors = action.GetAllSelectionAncestors();

            Assert.Equal([action], ancestors);
        }

        [Fact]
        public void GivenSelectAction_WhenMultipleSelectionAncestors_ThenReturned()
        {
            var project = new Project();

            var root = new ForeachAction();
            var select1 = root.AddChildAction(project, ActionType.Select);
            var child = select1.AddChildAction(project, ActionType.Foreach);
            var select2 = child.AddChildAction(project, ActionType.Select);
            child = select2.AddChildAction(project, ActionType.Foreach);
            var select3 = child.AddChildAction(project, ActionType.Select);

            var ancestors = select3.GetAllSelectionAncestors();

            Assert.Equal([select3, select2, select1], ancestors);
        }
    }
}
