using Liquidata.Common.Models;
using Microsoft.AspNetCore.Components;

namespace Liquidata.Client.Pages.Dialogs;

public class SelectionInfoDialogViewModel : DialogViewModelBase
{
    private static readonly char[] _pathSeparator = ['[', ']'];

    [Parameter]
    public SelectionInfo Info { get; set; } = null!;    

    protected IReadOnlyList<(string name, string index)> BuildPathTags()
    {
        var items = Info.FullPath.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var results = new List<(string name, string index)>();

        foreach (var item in items)
        {
            var parts = item.Split(_pathSeparator, StringSplitOptions.RemoveEmptyEntries);
            var name = parts[0];
            var index = parts.Length > 1
                ? parts[1]
                : string.Empty;            

            results.Add((name, index));
        }

        return results;
    }

    protected IReadOnlyList<(string name, string value)> BuildAttributes()
    {        
        var results = new List<(string name, string value)>();

        foreach (var attribute in Info.Attributes)
        {
            var items = attribute.Split(':', 2);
            results.Add((items[0], items[1]));
        }

        return results;
    }
}
