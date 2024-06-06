using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Actions.Shared;
using Liquidata.Common.Services.Interfaces;

namespace Liquidata.Client.Services.Interfaces;

public interface IClientBrowserService : IBrowserService
{
    Task UpdateBrowserSelectionModeAsync(ActionBase? action, BrowserMode browserMode);
    Task ClearCurrentSelectionsAsync();
    Task HighlightSelectionsAsync(string?[] selections);
    Task HighlightRelativeSelectionsAsync(string?[] selections);
    Task HighlightRelativeSelectionParentAsync(string? selection);
}
