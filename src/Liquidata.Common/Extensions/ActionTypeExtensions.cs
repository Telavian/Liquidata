using Liquidata.Common.Actions;
using Liquidata.Common.Actions.Enums;
using System.Text;

namespace Liquidata.Common.Extensions
{
    public static class ActionTypeExtensions
    {
        public static bool IsTemplateAction(this ActionType actionType)
        {
            return actionType is ActionType.Template;
        }

        public static bool IsSelectionAction(this ActionType actionType)
        {
            return actionType is ActionType.Select or 
                                 ActionType.RelativeSelect;
        }

        public static bool IsDataAction(this ActionType actionType)
        {
            return actionType is ActionType.BeginRecord or 
                                 ActionType.Extract or 
                                 ActionType.Log or 
                                 ActionType.Scope or 
                                 ActionType.ScreenCapture or 
                                 ActionType.Store;
        }

        public static bool IsLogicAction(this ActionType actionType)
        {
            return actionType is ActionType.Conditional or
                                 ActionType.ExecuteTemplate or
                                 ActionType.Foreach or
                                 ActionType.Jump or
                                 ActionType.JumpTarget;
        }

        public static bool IsInteractionAction(this ActionType actionType)
        {
            return actionType is ActionType.Click or
                                 ActionType.ExecuteScript or
                                 ActionType.Hover or
                                 ActionType.Input or
                                 ActionType.Keypress or
                                 ActionType.Reload or
                                 ActionType.Scroll or
                                 ActionType.SolveCaptcha or
                                 ActionType.Stop or
                                 ActionType.StopIf or
                                 ActionType.Wait;
        }

        public static ActionBase CreateNewAction(this ActionType actionType)
        {
            return actionType switch
            {
                ActionType.Template => new Template(),

                ActionType.Select => new SelectAction(),
                ActionType.RelativeSelect => new RelativeSelectAction(),

                ActionType.BeginRecord => new BeginRecordAction(),
                ActionType.Extract => new ExtractAction(),
                ActionType.Log => new LogAction(),
                ActionType.Scope => new ScopeAction(),
                ActionType.ScreenCapture => new ScreenCaptureAction(),
                ActionType.Store => new StoreAction(),

                ActionType.Conditional => new ConditionalAction(),
                ActionType.ExecuteTemplate => new ExecuteTemplateAction(),
                ActionType.Foreach => new ForeachAction(),
                ActionType.Jump => new JumpAction(),
                ActionType.JumpTarget => new JumpTargetAction(),

                ActionType.Click => new ClickAction(),
                ActionType.ExecuteScript => new ExecuteScriptAction(),
                ActionType.Hover => new HoverAction(),
                ActionType.Input => new InputAction(),
                ActionType.Keypress => new KeypressAction(),
                ActionType.Reload => new ReloadAction(),
                ActionType.Scroll => new ScrollAction(),
                ActionType.SolveCaptcha => new SolveCaptchaAction(),
                ActionType.Stop => new StopAction(),
                ActionType.StopIf => new StopIfAction(),
                ActionType.Wait => new WaitAction(),

                _ => throw new Exception($"Unknown action type '{actionType}'")
            };
        }
    }
}
