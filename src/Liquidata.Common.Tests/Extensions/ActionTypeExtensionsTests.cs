using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Extensions;

namespace Liquidata.Common.Tests.Extensions
{
    public class ActionTypeExtensionsTests
    {
        [Theory]
        [InlineData(ActionType.Template, true)]

        [InlineData(ActionType.Select, false)]
        [InlineData(ActionType.RelativeSelect, false)]

        [InlineData(ActionType.BeginRecord, false)]
        [InlineData(ActionType.Extract, false)]
        [InlineData(ActionType.Log, false)]
        [InlineData(ActionType.Scope, false)]
        [InlineData(ActionType.ScreenCapture, false)]
        [InlineData(ActionType.Store, false)]

        [InlineData(ActionType.Conditional, false)]
        [InlineData(ActionType.ExecuteTemplate, false)]
        [InlineData(ActionType.Foreach, false)]
        [InlineData(ActionType.Loop, false)]

        [InlineData(ActionType.Click, false)]
        [InlineData(ActionType.ExecuteScript, false)]
        [InlineData(ActionType.Hover, false)]
        [InlineData(ActionType.Input, false)]
        [InlineData(ActionType.Keypress, false)]
        [InlineData(ActionType.Reload, false)]
        [InlineData(ActionType.Scroll, false)]
        [InlineData(ActionType.SolveCaptcha, false)]
        [InlineData(ActionType.Stop, false)]
        [InlineData(ActionType.StopIf, false)]
        [InlineData(ActionType.Wait, false)]
        public void GivenAction_WhenTemplate_ThenIsTemplateAction(ActionType action, bool expected)
        {
            var isTemplate = action.IsTemplateAction();
            var actionItem = action.CreateNewAction();
            var actionIsTemplate = actionItem.IsTemplateAction();

            Assert.Equal(expected, isTemplate);
            Assert.Equal(expected, actionIsTemplate);
        }

        [Theory]
        [InlineData(ActionType.Template, false)]

        [InlineData(ActionType.Select, true)]
        [InlineData(ActionType.RelativeSelect, true)]

        [InlineData(ActionType.BeginRecord, false)]
        [InlineData(ActionType.Extract, false)]
        [InlineData(ActionType.Log, false)]
        [InlineData(ActionType.Scope, false)]
        [InlineData(ActionType.ScreenCapture, false)]
        [InlineData(ActionType.Store, false)]

        [InlineData(ActionType.Conditional, false)]
        [InlineData(ActionType.ExecuteTemplate, false)]
        [InlineData(ActionType.Foreach, false)]
        [InlineData(ActionType.Loop, false)]

        [InlineData(ActionType.Click, false)]
        [InlineData(ActionType.ExecuteScript, false)]
        [InlineData(ActionType.Hover, false)]
        [InlineData(ActionType.Input, false)]
        [InlineData(ActionType.Keypress, false)]
        [InlineData(ActionType.Reload, false)]
        [InlineData(ActionType.Scroll, false)]
        [InlineData(ActionType.SolveCaptcha, false)]
        [InlineData(ActionType.Stop, false)]
        [InlineData(ActionType.StopIf, false)]
        [InlineData(ActionType.Wait, false)]
        public void GivenAction_WhenSelection_ThenIsSelectionAction(ActionType action, bool expected)
        {
            var isTemplate = action.IsSelectionAction();
            var actionItem = action.CreateNewAction();
            var actionIsTemplate = actionItem.IsSelectionAction();

            Assert.Equal(expected, isTemplate);
            Assert.Equal(expected, actionIsTemplate);
        }

        [Theory]
        [InlineData(ActionType.Template, false)]

        [InlineData(ActionType.Select, false)]
        [InlineData(ActionType.RelativeSelect, false)]

        [InlineData(ActionType.BeginRecord, true)]
        [InlineData(ActionType.Extract, true)]
        [InlineData(ActionType.Log, true)]
        [InlineData(ActionType.Scope, true)]
        [InlineData(ActionType.ScreenCapture, true)]
        [InlineData(ActionType.Store, true)]

        [InlineData(ActionType.Conditional, false)]
        [InlineData(ActionType.ExecuteTemplate, false)]
        [InlineData(ActionType.Foreach, false)]
        [InlineData(ActionType.Loop, false)]

        [InlineData(ActionType.Click, false)]
        [InlineData(ActionType.ExecuteScript, false)]
        [InlineData(ActionType.Hover, false)]
        [InlineData(ActionType.Input, false)]
        [InlineData(ActionType.Keypress, false)]
        [InlineData(ActionType.Reload, false)]
        [InlineData(ActionType.Scroll, false)]
        [InlineData(ActionType.SolveCaptcha, false)]
        [InlineData(ActionType.Stop, false)]
        [InlineData(ActionType.StopIf, false)]
        [InlineData(ActionType.Wait, false)]
        public void GivenAction_WhenData_ThenIsDataAction(ActionType action, bool expected)
        {
            var isTemplate = action.IsDataAction();
            var actionItem = action.CreateNewAction();
            var actionIsTemplate = actionItem.IsDataAction();

            Assert.Equal(expected, isTemplate);
            Assert.Equal(expected, actionIsTemplate);
        }

        [Theory]
        [InlineData(ActionType.Template, false)]

        [InlineData(ActionType.Select, false)]
        [InlineData(ActionType.RelativeSelect, false)]

        [InlineData(ActionType.BeginRecord, false)]
        [InlineData(ActionType.Extract, false)]
        [InlineData(ActionType.Log, false)]
        [InlineData(ActionType.Scope, false)]
        [InlineData(ActionType.ScreenCapture, false)]
        [InlineData(ActionType.Store, false)]

        [InlineData(ActionType.Conditional, true)]
        [InlineData(ActionType.ExecuteTemplate, true)]
        [InlineData(ActionType.Foreach, true)]
        [InlineData(ActionType.Loop, true)]

        [InlineData(ActionType.Click, false)]
        [InlineData(ActionType.ExecuteScript, false)]
        [InlineData(ActionType.Hover, false)]
        [InlineData(ActionType.Input, false)]
        [InlineData(ActionType.Keypress, false)]
        [InlineData(ActionType.Reload, false)]
        [InlineData(ActionType.Scroll, false)]
        [InlineData(ActionType.SolveCaptcha, false)]
        [InlineData(ActionType.Stop, false)]
        [InlineData(ActionType.StopIf, false)]
        [InlineData(ActionType.Wait, false)]
        public void GivenAction_WhenLogic_ThenIsLogicAction(ActionType action, bool expected)
        {
            var isTemplate = action.IsLogicAction();
            var actionItem = action.CreateNewAction();
            var actionIsTemplate = actionItem.IsLogicAction();

            Assert.Equal(expected, isTemplate);
            Assert.Equal(expected, actionIsTemplate);
        }

        [Theory]
        [InlineData(ActionType.Template, false)]

        [InlineData(ActionType.Select, false)]
        [InlineData(ActionType.RelativeSelect, false)]

        [InlineData(ActionType.BeginRecord, false)]
        [InlineData(ActionType.Extract, false)]
        [InlineData(ActionType.Log, false)]
        [InlineData(ActionType.Scope, false)]
        [InlineData(ActionType.ScreenCapture, false)]
        [InlineData(ActionType.Store, false)]

        [InlineData(ActionType.Conditional, false)]
        [InlineData(ActionType.ExecuteTemplate, false)]
        [InlineData(ActionType.Foreach, false)]
        [InlineData(ActionType.Loop, false)]

        [InlineData(ActionType.Click, true)]
        [InlineData(ActionType.ExecuteScript, true)]
        [InlineData(ActionType.Hover, true)]
        [InlineData(ActionType.Input, true)]
        [InlineData(ActionType.Keypress, true)]
        [InlineData(ActionType.Reload, true)]
        [InlineData(ActionType.Scroll, true)]
        [InlineData(ActionType.SolveCaptcha, true)]
        [InlineData(ActionType.Stop, true)]
        [InlineData(ActionType.StopIf, true)]
        [InlineData(ActionType.Wait, true)]
        public void GivenAction_WhenInteraction_ThenIsInteractionAction(ActionType action, bool expected)
        {
            var isTemplate = action.IsInteractionAction();
            var actionItem = action.CreateNewAction();
            var actionIsTemplate = actionItem.IsInteractionAction();

            Assert.Equal(expected, isTemplate);
            Assert.Equal(expected, actionIsTemplate);
        }

        [Theory]
        [InlineData(ActionType.Template)]

        [InlineData(ActionType.Select)]
        [InlineData(ActionType.RelativeSelect)]

        [InlineData(ActionType.BeginRecord)]
        [InlineData(ActionType.Extract)]
        [InlineData(ActionType.Log)]
        [InlineData(ActionType.Scope)]
        [InlineData(ActionType.ScreenCapture)]
        [InlineData(ActionType.Store)]

        [InlineData(ActionType.Conditional)]
        [InlineData(ActionType.ExecuteTemplate)]
        [InlineData(ActionType.Foreach)]
        [InlineData(ActionType.Loop)]

        [InlineData(ActionType.Click)]
        [InlineData(ActionType.ExecuteScript)]
        [InlineData(ActionType.Hover)]
        [InlineData(ActionType.Input)]
        [InlineData(ActionType.Keypress)]
        [InlineData(ActionType.Reload)]
        [InlineData(ActionType.Scroll)]
        [InlineData(ActionType.SolveCaptcha)]
        [InlineData(ActionType.Stop)]
        [InlineData(ActionType.StopIf)]
        [InlineData(ActionType.Wait)]
        public void GivenAction_WhenAction_ThenActionTypeMatch(ActionType action)
        {            
            var actionItem = action.CreateNewAction();         

            Assert.Equal(action, actionItem.ActionType);
        }
    }
}
