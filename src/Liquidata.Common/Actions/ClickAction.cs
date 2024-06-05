using Liquidata.Common.Actions.Shared;
using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Extensions;
using System.Text.Json.Serialization;

namespace Liquidata.Common.Actions;

public class ClickAction : ActionBase
{    
    [JsonIgnore] public override ActionType ActionType => ActionType.Click;
    [JsonIgnore] public override bool AllowChildren => false;
    [JsonIgnore] public override bool IsInteractive => true;

    public ClickType ClickType { get; set; } = ClickType.Continue;
    public Guid? ExecutionTemplateId { get; set; } = null!;
    public ClickButton ClickButton { get; set; } = ClickButton.Left;
    public bool IsDoubleClick { get; set; }
    public int WaitMilliseconds { get; set; }
    public int MaxTimesCount { get; set; } = 1;
    public bool IsMaxTimesTemplate { get; set; } = true;

    public override string[] BuildValidationErrors()
    {
        return ClickType == ClickType.ExecuteTemplate && ExecutionTemplateId.IsNotDefined() 
            ? (["No template selected"]) 
            : ([]);
    }
}
