using Liquidata.Common.Actions.Enums;

namespace Liquidata.Common.Extensions;

public static class ScriptTypeExtensions
{
    public static string BuildScript(this ScriptType scriptType)
    {
        return scriptType switch
        {
            ScriptType.Custom => "",
            ScriptType.Alt => "return $this.getAttr('alt');",
            ScriptType.Class => "return $this.getAttr('class');",
            ScriptType.Height => "return $this.getHeight();",
            ScriptType.Link => "return $this.getLink();",
            ScriptType.PageUrl => "return $this.getPageUrl();",
            ScriptType.Source => "return $this.getAttr('src');",
            ScriptType.StarRating => "return $this.getStarRating('class', 'full', 'half');",
            ScriptType.Text => "return $this.getText();",
            ScriptType.Time => "return $this.getTime();",
            ScriptType.Title => "return $this.getAttr('title');",
            ScriptType.Width => "return $this.getWidth();",
            _ => throw new Exception($"Unknown script type: {scriptType}")
        };
    }

    public static ScriptType GetMatchingScriptType(this string? script)
    {
        if (string.IsNullOrWhiteSpace(script))
        {
            return ScriptType.Custom;
        }

        foreach (var scriptType in Enum.GetValues<ScriptType>())
        {
            if (scriptType.BuildScript() == script)
            {
                return scriptType;
            }
        }

        return ScriptType.Custom;
    }
}
