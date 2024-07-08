using Liquidata.Common.Extensions;

namespace Liquidata.Common.Tests.Extensions;

public class JsonExtensionsTests
{
    [Fact]
    public void GivenClass_WhenToJson_ThenJsonCreated()
    {
        var item = new TestClass { Prop1 = 123, Prop2 = Guid.NewGuid().ToString() };
        var json = item.ToJson();

        Assert.StartsWith("{", json);
        Assert.EndsWith("}", json);
        Assert.Contains(item.Prop1.ToString(), json);
        Assert.Contains(item.Prop2, json);
    }

    [Fact]
    public void GivenClass_WhenFromJson_ThenItemCreated()
    {
        var item = new TestClass { Prop1 = 123, Prop2 = Guid.NewGuid().ToString() };
        var json = item.ToJson();
        var created = json.FromJson<TestClass>()!;

        Assert.Equal(item.Prop1, created.Prop1);
        Assert.Equal(item.Prop2, created.Prop2);
    }

    [Fact]
    public async Task GivenClass_WhenFromJsonStream_ThenItemCreated()
    {
        var item = new TestClass { Prop1 = 123, Prop2 = Guid.NewGuid().ToString() };
        var json = item.ToJson();

        using var memStream = new MemoryStream();
        using var writer = new StreamWriter(memStream, leaveOpen: true);
        await writer.WriteAsync(json);
        await writer.FlushAsync();
        writer.Close();

        memStream.Position = 0;
        var created = await memStream.FromJsonAsync<TestClass>();

        Assert.Equal(item.Prop1, created!.Prop1);
        Assert.Equal(item.Prop2, created.Prop2);
    }
}

class TestClass
{
    public int Prop1 { get; set; } = 0;
    public string Prop2 { get; set; } = "";
}
