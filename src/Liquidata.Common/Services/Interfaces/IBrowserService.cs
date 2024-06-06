using Liquidata.Common.Models;

namespace Liquidata.Common.Services.Interfaces;

public interface IBrowserService
{        
    Task InitializeBrowserAsync();
    Task<bool> CheckIfWebSecurityEnabledAsync();
    Task<bool> WaitForBrowserReadyAsync(TimeSpan waitTime);
    Task<SelectionInfo> GetSelectionInfoAsync(string xpath);
    Task<string[]> GetAllMatchesAsync(string xpath);
}
