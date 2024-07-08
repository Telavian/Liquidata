using System.Reflection;
using System.Xml.Linq;
using Liquidata.Common.Extensions;

namespace Liquidata.Common.Tests.Extensions;

public class ResourceExtensionsTests
{
    private readonly string _testResourceName = "Liquidata.Common.Tests.Resources.TestResource.txt";

    [Fact]
    public async Task GivenResource_WhenLoadResource_ThenLoaded()
    {
        var result = await Assembly.GetExecutingAssembly()
            .LoadResourceAsync(_testResourceName);

        Assert.Equal("12345", result);
    }

    [Fact]
    public async Task GivenResource_WhenInvalid_ThenException()
    {
        var name = $"{_testResourceName}2";
        var exception = await Assert.ThrowsAsync<Exception>(async () => await Assembly.GetExecutingAssembly().LoadResourceAsync(name));
        Assert.Equal($"Resource '{name}' not found", exception.Message);
    }
}
