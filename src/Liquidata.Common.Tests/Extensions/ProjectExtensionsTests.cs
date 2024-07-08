using Liquidata.Common.Extensions;

namespace Liquidata.Common.Tests.Extensions;

public class ProjectExtensionsTests
{
    [Fact]
    public async Task GivenCall_WhenNullProject_ThenException()
    {
        FileInfo? file = null;

        var exception = await Assert.ThrowsAsync<Exception>(() => file.LoadProjectAsync());
        Assert.Equal("Project file is not defined", exception.Message);
    }

    [Fact]
    public async Task GivenCall_WhenLoadProject_ThenProjectLoaded()
    {
        var tempFile = Path.GetTempFileName();

        try
        {
            var project = new Project
            {
                Name = Guid.NewGuid().ToString(),
                ProjectId = Guid.NewGuid(),
                Url = Guid.NewGuid().ToString(),
            };

            var file = new FileInfo(tempFile);
            var json = project.ToJson();
            await File.WriteAllTextAsync(tempFile, json);

            var loaded = await file.LoadProjectAsync();

            Assert.NotNull(loaded);
            Assert.Equal(project.Name, loaded.Name);
            Assert.Equal(project.ProjectId, loaded.ProjectId);
            Assert.Equal(project.Url, loaded.Url);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }
}
