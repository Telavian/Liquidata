using Liquidata.UI.Common.Pages.Common;
using Microsoft.AspNetCore.Components;
using Liquidata.Common.Extensions;
using Liquidata.Client.Services.Interfaces;
using Liquidata.Client.Emporium.Models;

namespace Liquidata.Client.Emporium.Pages;

public partial class EmporiumViewModel : ViewModelBase
{
    [Inject] private IEmporiumService _emporiumService { get; set; } = null!;

    public const string NavigationPath = "Emporium";

    public bool IsGeneratingData { get; set; }
    public int GeneratedCount { get; set; }
    public int GeneratedTotal { get; set; }

    public EmporiumData EmporiumData { get; set; } = null!;
    public EmporiumFeaturedCategory[] FeaturedCategories { get; set; } = [];

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        _ = InitializeDataAsync();
    }

    private async Task InitializeDataAsync()
    {
        await GenerateDataAsync();
        await BuildFeaturedCategoriesAsync();
    }

    private async Task GenerateDataAsync()
    {
        var data = await _emporiumService.GenerateDataAsync(
            async () =>
            {
                IsGeneratingData = true;
                await RefreshAsync();
            },
            async (count, total) =>
            {
                GeneratedCount = count;
                GeneratedTotal = total;
                await RefreshAsync();
                await Task.Delay(1);
            });

        IsGeneratingData = false;
        EmporiumData = data;
        await RefreshAsync();
    }

    private async Task BuildFeaturedCategoriesAsync()
    {
        await Task.Yield();

        var categories = EmporiumData.AllCategories.TakeRandom(3);

        FeaturedCategories = categories
            .Select(x => new EmporiumFeaturedCategory
            {
                Category = x,
                Items = EmporiumData.AllItems
                    .Where(y => y.Category.Name == x.Name)
                    .ToArray()
                    .TakeRandom(4),
            })
            .ToArray();

        await RefreshAsync();
    }
}
