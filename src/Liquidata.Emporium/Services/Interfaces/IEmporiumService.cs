using Liquidata.Emporium.Models;

namespace Liquidata.Emporium.Services.Interfaces;

public interface IEmporiumService
{
    Task<EmporiumData?> LoadDataAsync();
    Task<EmporiumData> GenerateDataAsync(Func<Task> initialAction, Func<int, int, Task> refreshAction);
    Task<EmporiumItem> LoadDataItem(Guid productId);
}
