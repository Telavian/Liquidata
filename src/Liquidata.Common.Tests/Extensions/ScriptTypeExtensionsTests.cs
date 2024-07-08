using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Extensions;

namespace Liquidata.Common.Tests.Extensions;

public class ScriptTypeExtensionsTests
{
    [Theory]
    [InlineData(ScriptType.Custom, "")]
    [InlineData(ScriptType.Alt, "return $this.getAttr('alt');")]
    [InlineData(ScriptType.Class, "return $this.getAttr('class');")]
    [InlineData(ScriptType.Height, "return $this.getHeight();")]
    [InlineData(ScriptType.Link, "return $this.getLink();")]
    [InlineData(ScriptType.PageUrl, "return $this.getPageUrl();")]
    [InlineData(ScriptType.Source, "return $this.getAttr('src');")]
    [InlineData(ScriptType.Text, "return $this.getText();")]
    [InlineData(ScriptType.Time, "return $this.getTime();")]
    [InlineData(ScriptType.Title, "return $this.getAttr('title');")]
    [InlineData(ScriptType.Width, "return $this.getWidth();")]
    public void GivenScriptType_WhenBuildScript_ThenValid(ScriptType scriptType, string expected)
    {
        var script = scriptType.BuildScript();
        Assert.Equal(expected, script);
    }

    [Fact]
    public void GivenScriptType_WhenInvalidScriptType_ThenException()
    {
        var scriptType = ((ScriptType)292992);
        var error = Assert.Throws<Exception>(() => scriptType.BuildScript());
        
        Assert.Contains("Unknown script type", error.Message);
    }
}
