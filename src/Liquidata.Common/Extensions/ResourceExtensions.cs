using System.Reflection;

namespace Liquidata.Common.Extensions;

public static class ResourceExtensions
{
    public static async Task<string> LoadResourceAsync(this Assembly assembly, string name)
    {
        using var stream = assembly
            .GetManifestResourceStream(name);

        if (stream is null)
        {
            throw new Exception($"Resource '{name}' not found");
        }

        using var reader = new StreamReader(stream);
        var result = await reader.ReadToEndAsync();

        return result;
    }
}
