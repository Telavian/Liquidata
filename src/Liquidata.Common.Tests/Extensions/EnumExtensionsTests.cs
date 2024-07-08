using Liquidata.Common.Extensions;

namespace Liquidata.Common.Tests.Extensions;

public class EnumExtensionsTests
{
    [Fact]
    public void GiveEnum_WhenLowercase_ThenTextLower()
    {
        var result = TestEnum.TestEnumValue.BuildFriendlyName(true);
        Assert.Equal("test enum value", result);
    }

    [Fact]
    public void GiveEnum_WhenUppercase_ThenTextUpper()
    {
        var result = TestEnum.TestEnumValue.BuildFriendlyName();
        Assert.Equal("Test Enum Value", result);
    }
}

enum TestEnum
{
    TestEnumValue,
}
