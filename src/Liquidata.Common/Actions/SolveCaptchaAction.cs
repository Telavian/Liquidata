using Liquidata.Common.Actions.Enums;
using System.Text.Json.Serialization;

namespace Liquidata.Common.Actions;

public class SolveCaptchaAction : ActionBase
{
    [JsonIgnore] public override ActionType ActionType => ActionType.SolveCaptcha;
    [JsonIgnore] public override bool AllowChildren => false;

    public int WaitMilliseconds { get; set; }
    public int MaxTimesCount { get; set; } = 1;
    public bool IsMaxTimesTemplate { get; set; } = true;

    public override string[] BuildValidationErrors()
    {
        return [];
    }
}
