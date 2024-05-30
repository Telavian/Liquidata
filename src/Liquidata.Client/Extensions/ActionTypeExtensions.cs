using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Extensions;
using MudBlazor;

namespace Liquidata.Client.Extensions
{
    public static class ActionTypeExtensions
    {
        public static string BuildActionToolip(this ActionType actionType)
        {
            return actionType switch
            {
                ActionType.Template => "Set of reusable actions",

                ActionType.Select => "Select items for interaction",
                ActionType.RelativeSelect => "Select items relative to a selection",

                ActionType.BeginRecord => "Begin a new extraction record",
                ActionType.Extract => "Extract data item",
                ActionType.Log => "Log debugging message",
                ActionType.Scope => "Define scope to alter extracted data",
                ActionType.ScreenCapture => "Take screenshot of current page",
                ActionType.Store => "Store value for later use",

                ActionType.Conditional => "Define logic to conditionally execute actions",
                ActionType.ExecuteTemplate => "Executes the action template",
                ActionType.Foreach => "Executes actions for each item",
                ActionType.Jump => "Jump to another local in current temple",
                ActionType.JumpTarget => "Defines location that can be jumped to",

                ActionType.Click => "Click on current selection",
                ActionType.ExecuteScript => "Execute the defined script",
                ActionType.Hover => "Hover over current selection",
                ActionType.Input => "Input information to current selection",
                ActionType.Keypress => "Send keypress to current selection",
                ActionType.Reload => "Reloads the current page",
                ActionType.Scroll => "Scroll the current page",
                ActionType.SolveCaptcha => "Solve captcha problem",
                ActionType.Stop => "Stop parser execution",
                ActionType.StopIf => "Conditionally stop parser execution",
                ActionType.Wait => "Wait a defined amount of time",

                _ => throw new Exception($"Unknown action type '{actionType}'")
            };
        }

        public static string BuildActionIcon(this ActionType actionType)
        {
            return actionType switch
            {
                ActionType.Template => Icons.Material.TwoTone.Dataset,

                ActionType.Select => Icons.Material.TwoTone.SelectAll,
                ActionType.RelativeSelect => Icons.Material.TwoTone.MoveDown,

                ActionType.BeginRecord => Icons.Material.TwoTone.Start,
                ActionType.Extract => Icons.Material.TwoTone.DataObject,
                ActionType.Log => Icons.Material.TwoTone.LogoDev,
                ActionType.Scope => Icons.Material.TwoTone.MoveToInbox,
                ActionType.ScreenCapture => Icons.Material.TwoTone.ScreenshotMonitor,
                ActionType.Store => Icons.Material.TwoTone.Save,

                ActionType.Conditional => Icons.Material.TwoTone.AccountTree,
                ActionType.ExecuteTemplate => Icons.Material.TwoTone.DatasetLinked,
                ActionType.Foreach => Icons.Material.TwoTone.Loop,
                ActionType.Jump => Icons.Material.TwoTone.LocationSearching,
                ActionType.JumpTarget => Icons.Material.TwoTone.MyLocation,

                ActionType.Click => Icons.Material.TwoTone.AdsClick,
                ActionType.ExecuteScript => Icons.Material.TwoTone.Code,
                ActionType.Hover => Icons.Material.TwoTone.Mouse,
                ActionType.Input => Icons.Material.TwoTone.Input,
                ActionType.Keypress => Icons.Material.TwoTone.Keyboard,
                ActionType.Reload => Icons.Material.TwoTone.Downloading,
                ActionType.Scroll => Icons.Material.TwoTone.Expand,
                ActionType.SolveCaptcha => Icons.Material.TwoTone.Calculate,
                ActionType.Stop => Icons.Material.TwoTone.StopCircle,
                ActionType.StopIf => Icons.Material.TwoTone.Pending,
                ActionType.Wait => Icons.Material.TwoTone.HourglassEmpty,

                _ => throw new Exception($"Unknown action type '{actionType}'")
            };
        }

        public static Color BuildActionColor(this ActionType actionType)
        {
            if (actionType == ActionType.Template)
            {
                return Color.Default;
            }

            if (actionType.IsSelectionAction())
            {
                return Color.Secondary;
            }

            if (actionType.IsDataAction())
            {
                return Color.Primary;
            }

            if (actionType.IsLogicAction())
            {
                return Color.Success;
            }

            if (actionType.IsInteractionAction())
            {
                return Color.Info;
            }

            throw new Exception($"Unknown action type '{actionType}'");
        }
    }
}
