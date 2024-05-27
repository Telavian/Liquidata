namespace Liquidata.Common.Actions;

public class SolveCaptchaAction : ActionBase
{
    public override ActionType ActionType => ActionType.SolveCaptcha;
    public override bool AllowChildren => false;

    public int WaitMilliseconds { get; set; }
    public int MaxTimesCount { get; set; } = 1;
    public bool IsMaxTimesTemplate { get; set; } = true;

    public override string[] BuildValidationErrors()
    {
        return [];
    }
}
