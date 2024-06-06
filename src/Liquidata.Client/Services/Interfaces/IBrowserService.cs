using Liquidata.Client.Models;
using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Actions.Shared;

namespace Liquidata.Client.Services.Interfaces
{
    public interface IBrowserService
    {
        Task UpdateBrowserSelectionModeAsync(ActionBase? action, BrowserMode browserMode);
        Task InitializeBrowserAsync();
        Task<bool> CheckIfWebSecurityEnabledAsync();
        Task<bool> WaitForBrowserReadyAsync(TimeSpan waitTime);
        Task ClearCurrentSelectionsAsync();
        Task HighlightSelectionsAsync(string?[] selections);
        Task HighlightRelativeSelectionsAsync(string?[] selections);
        Task HighlightRelativeSelectionParentAsync(string? selection);
        Task<SelectionInfo> GetSelectionInfoAsync(string xpath);
        Task<string[]> GetAllMatchesAsync(string xpath);
    }
}
