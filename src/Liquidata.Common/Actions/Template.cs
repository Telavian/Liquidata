namespace Liquidata.Common.Actions;

public class Template : ActionBase
{
    public const string MainTemplateName = "main";

    public override ActionType ActionType => ActionType.Template;
    public override bool AllowChildren => true;

    public string Url { get; set; } = "";    

    public ICollection<JumpTargetAction> FindAllJumpTargets()
    {
        return FindActions(x => x.ActionType == ActionType.JumpTarget)
            .OfType<JumpTargetAction>()
            .ToList();
    }
}
