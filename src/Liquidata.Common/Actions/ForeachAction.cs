﻿using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Extensions;
using System.Text.Json.Serialization;

namespace Liquidata.Common.Actions;

public class ForeachAction : ActionBase
{
    [JsonIgnore] public override ActionType ActionType => ActionType.Foreach;
    [JsonIgnore] public override bool AllowChildren => true;

    public ScriptType ScriptType { get; set; }
    public string? Script { get; set; } = null!;

    public override string[] BuildValidationErrors()
    {
        var errors = new List<string>();

        if (Name.IsNotDefined())
        {
            errors.Add("Name is not defined");
        }

        if (Script.IsNotDefined())
        {
            errors.Add("Script is not defined");
        }

        return errors.ToArray();
    }
}
