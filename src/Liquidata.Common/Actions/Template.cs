using Liquidata.Common.Actions.Enums;
using System.Text.Json.Serialization;

namespace Liquidata.Common.Actions;

public class Template : ActionBase
{
    public const string MainTemplateName = "main";

    [JsonIgnore] public override ActionType ActionType => ActionType.Template;
    [JsonIgnore] public override bool AllowChildren => true;

    public string Url { get; set; } = "";

    public override string[] BuildValidationErrors()
    {
        // TODO: Url required?
        return [];
    }

    public ICollection<JumpTargetAction> FindAllJumpTargets()
    {
        return FindActions(x => x.ActionType == ActionType.JumpTarget)
            .OfType<JumpTargetAction>()
            .ToList();
    }
}
