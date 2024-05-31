using Liquidata.Common.Actions.Enums;
using System.Text.Json.Serialization;

namespace Liquidata.Common.Actions;

public class RelativeSelectAction : SelectionActionBase
{
    [JsonIgnore] public override ActionType ActionType => ActionType.RelativeSelect;       
}
