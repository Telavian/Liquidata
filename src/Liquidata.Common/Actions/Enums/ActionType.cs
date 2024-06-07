namespace Liquidata.Common.Actions.Enums;

public enum ActionType
{
    Unknown = 0,

    // Template
    Template,

    // Selection
    Select,
    RelativeSelect,

    //Data
    BeginRecord,
    Extract,
    Log,
    Scope,
    ScreenCapture,
    Store,

    // Logic
    Conditional,
    ExecuteTemplate,
    Foreach,
    Loop,

    // Interaction
    Click,
    ExecuteScript,
    Hover,
    Input,
    Keypress,
    Reload,
    Scroll,
    SolveCaptcha,
    Stop,
    StopIf,
    Wait
}
