using Liquidata.Common.Actions.Enums;

namespace Liquidata.Common.Extensions
{
    public static class ScriptTypeExtensions
    {
        public static string BuildScript(this ScriptType scriptType)
        {
            return scriptType switch
            {
                ScriptType.Custom => "",
                ScriptType.Alt => "$this.getAttr('alt')",
                ScriptType.Class => "$this.getAttr('class')",
                ScriptType.Height => "$this.getHeight()",
                ScriptType.Link => "$this.getLink()",
                ScriptType.PageUrl => "$this.getPageUrl()",
                ScriptType.Size => "$this.getSize()",
                ScriptType.Source => "$this.getAttr('src')",
                ScriptType.StarRating => "$this.getStarRating('class', 'full', 'half')",
                ScriptType.Text => "$this.getText()",
                ScriptType.Time => "$this.getTime()",
                ScriptType.Title => "$this.getAttr('title')",
                ScriptType.Width => "$this.getWidth()",
                _ => throw new Exception($"Unknown script type: {scriptType}")
            };
        }
    }
}
