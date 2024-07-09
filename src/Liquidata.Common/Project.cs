using Liquidata.Common.Actions;
using Liquidata.Common.Extensions;
using Liquidata.Common.Services.Interfaces;
using System.Diagnostics;
using System.Text.Json;

namespace Liquidata.Common;

public class Project
{
    public Guid ProjectId { get; init; } = Guid.NewGuid();
    public string Name { get; set; } = null!;
    public string Url { get; set; } = null!;
    public int Concurrency { get; set; } = 3;

    public List<Template> AllTemplates { get; init; } = new List<Template>();

    public Project Clone(string name)
    {
        return new Project
        {
            Name = name,
            Url = Url,
            Concurrency = Concurrency,
            AllTemplates = AllTemplates
        };
    }

    public bool CheckIfInteractive()
    {
        return AllTemplates
            .SelectMany(x => x.TraverseTree())
            .Any(x => x.IsInteractive);
    }

    public bool CheckIfFullyDefined()
    {
        return !AllTemplates
            .SelectMany(x => x.TraverseTree())
            .SelectMany(x => x.BuildValidationErrors())
            .Any();
    }

    public Project FullClone()
    {
        var json = this.ToJson(); JsonSerializer.Serialize(this);        
        var result = json.FromJson<Project>()!;

        result.RestoreParentReferences();
        return result;
    }

    public void RestoreParentReferences()
    {
        foreach (var template in AllTemplates)
        {
            template.RestoreParentReferences(null);
        }
    }

    public async Task ExecuteProjectAsync(IExecutionService executionService)
    {
        await Task.Yield();
        var mainTemplate = AllTemplates.FirstOrDefault(x => x.Name == Template.MainTemplateName);

        if (mainTemplate is null)
        {
            throw new Exception("Unable to find main template");
        }

        var timer = Stopwatch.StartNew();
        Console.WriteLine($"Project '{Name}' execution started");

        await mainTemplate.ExecuteActionAsync(executionService);
        await executionService.WaitForExecutionTasksAsync();

        Console.WriteLine($"Project '{Name}' execution complete in {timer.Elapsed}");
    }
}
