namespace Liquidata.Common.Extensions;

public static class ProjectExtensions
{
    public static async Task<Project> LoadProjectAsync(this FileInfo? file)
    {
        if (file is null)
        {
            throw new Exception("Project file is not defined");
        }

        using var stream = file.OpenRead();
        var project = await stream.FromJsonAsync<Project>();

        return project 
            ?? throw new Exception("Unable to deserialize project file");
    }
}
