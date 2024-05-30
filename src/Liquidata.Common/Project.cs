using Liquidata.Common.Actions;

namespace Liquidata.Common;

public class Project
{
    public Guid ProjectId { get; init; } = Guid.NewGuid();
    public string Name { get; set; } = null!;
    public string Url { get; set; } = null!;
    public bool LoadImages { get; set; } = true;
    public bool RotateIpAddresses { get; set; } = false;
    public int Concurrency { get; set; } = 3;

    public List<Template> AllTemplates { get; init; } = new List<Template>();

    public void RestoreParentReferences()
    {
        foreach (var template in AllTemplates)
        {
            template.RestoreParentReferences(null);
        }
    }
}
