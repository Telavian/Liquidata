using Liquidata.Client.Emporium.Models;

namespace Liquidata.Client.Services.Interfaces;

public interface IEmporiumService
{
    Task<EmporiumData?> LoadDataAsync();
    Task<EmporiumData> GenerateDataAsync(Func<Task> initialAction, Func<int, int, Task> refreshAction);
    Task<EmporiumItem> LoadDataItemAsync(Guid productId, Func<Task> initialAction, Func<int, int, Task> refreshAction);
}
