using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Extensions;
using System.Text.Json.Serialization;

namespace Liquidata.Common.Actions.Shared;

[JsonDerivedType(typeof(Template), nameof(Template))]

[JsonDerivedType(typeof(SelectAction), nameof(SelectAction))]
[JsonDerivedType(typeof(RelativeSelectAction), nameof(RelativeSelectAction))]

[JsonDerivedType(typeof(BeginRecordAction), nameof(BeginRecordAction))]
[JsonDerivedType(typeof(ExtractAction), nameof(ExtractAction))]
[JsonDerivedType(typeof(LogAction), nameof(LogAction))]
[JsonDerivedType(typeof(ScopeAction), nameof(ScopeAction))]
[JsonDerivedType(typeof(ScreenCaptureAction), nameof(ScreenCaptureAction))]
[JsonDerivedType(typeof(StoreAction), nameof(StoreAction))]

[JsonDerivedType(typeof(ConditionalAction), nameof(ConditionalAction))]
[JsonDerivedType(typeof(ExecuteTemplateAction), nameof(ExecuteTemplateAction))]
[JsonDerivedType(typeof(ForeachAction), nameof(ForeachAction))]
[JsonDerivedType(typeof(JumpAction), nameof(JumpAction))]
[JsonDerivedType(typeof(JumpTargetAction), nameof(JumpTargetAction))]

[JsonDerivedType(typeof(ClickAction), nameof(ClickAction))]
[JsonDerivedType(typeof(ExecuteScriptAction), nameof(ExecuteScriptAction))]
[JsonDerivedType(typeof(HoverAction), nameof(HoverAction))]
[JsonDerivedType(typeof(InputAction), nameof(InputAction))]
[JsonDerivedType(typeof(KeypressAction), nameof(KeypressAction))]
[JsonDerivedType(typeof(ReloadAction), nameof(ReloadAction))]
[JsonDerivedType(typeof(ScrollAction), nameof(ScrollAction))]
[JsonDerivedType(typeof(SolveCaptchaAction), nameof(SolveCaptchaAction))]
[JsonDerivedType(typeof(StopAction), nameof(StopAction))]
[JsonDerivedType(typeof(StopIfAction), nameof(StopIfAction))]
[JsonDerivedType(typeof(WaitAction), nameof(WaitAction))]
public abstract class ActionBase
{
    [JsonIgnore] public abstract ActionType ActionType { get; }
    [JsonIgnore] public abstract bool AllowChildren { get; }
    [JsonIgnore] public abstract bool IsInteractive { get; }

    public Guid ActionId { get; init; } = Guid.NewGuid();
    public string Name { get; set; } = "";
    public bool IsDisabled { get; set; } = false;

    [JsonIgnore]
    public ActionBase? Parent { get; set; } = null!;
    public IReadOnlyCollection<ActionBase> ChildActions { get; set; } = new List<ActionBase>();

    public abstract string[] BuildValidationErrors();

    public ActionBase AddChildAction(ActionType actionType)
    {
        var action = actionType.CreateNewAction();
        action.Parent = this;

        ChildActions = (ChildActions ?? [])
            .Concat([action])
            .ToList();

        return action;
    }

    public ActionBase AddSiblingAction(ActionType actionType)
    {
        var action = actionType.CreateNewAction();
        action.Parent = Parent;

        var actionsList = (Parent!.ChildActions ?? [])
            .ToList();

        var index = actionsList.IndexOf(this);

        actionsList.Insert(index + 1, action);
        Parent!.ChildActions = actionsList;

        return action;
    }

    public void RestoreParentReferences(ActionBase? parent)
    {
        Parent = parent;

        foreach (var child in ChildActions ?? [])
        {
            child.RestoreParentReferences(this);
        }
    }

    public ICollection<ActionBase> FindActions(Func<ActionBase, bool> filter)
    {
        return FindActions(filter, new List<ActionBase>());
    }

    public void RemoveChild(ActionBase child)
    {
        var actions = (ChildActions ?? [])
            .ToList();

        actions.Remove(child);

        ChildActions = actions
            .ToList();
    }

    public override bool Equals(object? o)
    {
        var other = o as ActionBase;
        return other?.ActionId == ActionId;
    }

    public override int GetHashCode()
    {
        return ActionId.GetHashCode();
    }

    public override string ToString()
    {
        return Name;
    }

    private ICollection<ActionBase> FindActions(Func<ActionBase, bool> filter, List<ActionBase>? results = null)
    {
        results ??= new List<ActionBase>();

        var isMatch = filter(this);
        if (isMatch)
        {
            results.Add(this);
        }

        foreach (var child in ChildActions ?? [])
        {
            child.FindActions(filter, results);
        }

        return results;
    }
}
