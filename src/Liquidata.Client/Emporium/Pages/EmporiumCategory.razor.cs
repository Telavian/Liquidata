using Liquidata.Client.Emporium.Models;
using Liquidata.Client.Services.Interfaces;
using Liquidata.UI.Common.Pages.Common;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Liquidata.Client.Emporium.Pages;

public class EmporiumCategoryViewModel : ViewModelBase
{
    [Inject] private IEmporiumService _emporiumService { get; set; } = null!;

    [Parameter]
    [SupplyParameterFromQuery]
    public string Category { get; set; } = "";

    public EmporiumData? EmporiumData { get; set; } = null!;
    public EmporiumItem[] AllItems { get; set; } = [];
    public EmporiumItem[] FilteredItems { get; set; } = [];
    public string? SearchText { get; set; }
    public int ItemsPerPage { get; set; } = 5;

    public int TotalPages { get; set; }
    public EmporiumItem[] ItemsOnPage { get; set; } = [];

    private int _selectedPageIndex = 0;
    public int SelectedPageIndex
    {
        get => _selectedPageIndex;
        set
        {
            _selectedPageIndex = value;
            ItemsOnPage = FilteredItems
                .Skip(ItemsPerPage * (SelectedPageIndex - 1))
                .Take(ItemsPerPage)
                .ToArray();

            _ = RefreshAsync();
        }
    }

    private Func<Task>? _searchProductsAsyncCommand;
    public Func<Task> SearchProductsAsyncCommand => _searchProductsAsyncCommand ??= CreateEventCallbackAsyncCommand(HandleSearchProductsAsync, "Unable to search products");

    private Func<KeyboardEventArgs, Task>? _searchKeyPressedAsyncCommand;
    public Func<KeyboardEventArgs, Task> SearchKeyPressedAsyncCommand => _searchKeyPressedAsyncCommand ??= CreateEventCallbackAsyncCommand<KeyboardEventArgs>(HandleSearchKeyPressedAsync, "Unable to handle key pressed");

    public static string BuildNavigationLink(string category)
    {
        return $"Emporium/Category/{category}";
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        EmporiumData = await _emporiumService.LoadDataAsync();

        if (EmporiumData is null)
        {
            await NavigateToAsync($"{EmporiumViewModel.NavigationPath}");
            return;
        }

        AllItems = EmporiumData.AllItems
            .Where(x => string.Equals(x.Category.Name, Category, StringComparison.OrdinalIgnoreCase))
            .ToArray();
        await RefreshAsync();

        _ = SearchProductsAsyncCommand();
    }

    private async Task HandleSearchProductsAsync()
    {
        await Task.Yield();

        if (string.IsNullOrWhiteSpace(SearchText))
        {
            FilteredItems = AllItems;
        }
        else
        {
            FilteredItems = AllItems
                .Where(x =>
                    x.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    x.Manufacturer.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    x.Description.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                .ToArray();
        }

        TotalPages = (int)Math.Ceiling((double)FilteredItems.Length / ItemsPerPage);
        SelectedPageIndex = 1;
    }

    private async Task HandleSearchKeyPressedAsync(KeyboardEventArgs args)
    {
        if (args.Code == "Enter" || args.Code == "Return" || args.Code == "NumpadEnter")
        {
            await HandleSearchProductsAsync();
        }

        return;
    }
}
