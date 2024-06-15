using Bogus.Bson;
using Liquidata.Emporium.Models;
using Liquidata.Emporium.Services.Interfaces;
using Liquidata.UI.Common.Pages.Common;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Liquidata.Emporium.Pages;

public class DetailsViewModel : ViewModelBase
{
    [Inject] private IEmporiumService _emporiumService { get; set; } = null!;

    [Parameter]
    [SupplyParameterFromQuery]
    public string ProductId { get; set; } = "";

    public bool IsGeneratingData { get; set; }
    public int GeneratedCount { get; set; }
    public int GeneratedTotal { get; set; }

    public EmporiumData? EmporiumData { get; set; } = null!;
    public EmporiumItem? Product { get; set; } = null!;

    public EmporiumReview[] AllReviews { get; set; } = [];
    public EmporiumReview[] FilteredReviews { get; set; } = [];
    public string? SearchText { get; set; }
    public int ReviewsPerPage { get; set; } = 10;

    public int TotalPages { get; set; }
    public EmporiumReview[] ReviewsOnPage { get; set; } = [];

    private int _selectedPageIndex = 0;
    public int SelectedPageIndex
    {
        get => _selectedPageIndex;
        set
        {
            _selectedPageIndex = value;
            ReviewsOnPage = FilteredReviews
                .Skip(ReviewsPerPage * (SelectedPageIndex - 1))
                .Take(ReviewsPerPage)
                .ToArray();

            _ = RefreshAsync();
        }
    }

    private Func<Task>? _searchReviewsAsyncCommand;
    public Func<Task> SearchReviewsAsyncCommand => _searchReviewsAsyncCommand ??= CreateEventCallbackAsyncCommand(HandleSearchReviewsAsync, "Unable to search reviews");

    private Func<KeyboardEventArgs, Task>? _searchKeyPressedAsyncCommand;
    public Func<KeyboardEventArgs, Task> SearchKeyPressedAsyncCommand => _searchKeyPressedAsyncCommand ??= CreateEventCallbackAsyncCommand<KeyboardEventArgs>(HandleSearchKeyPressedAsync, "Unable to handle key pressed");

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        EmporiumData = await _emporiumService.LoadDataAsync();

        if (EmporiumData is null || !Guid.TryParse(ProductId, out var idValue))
        {
            await NavigateToAsync("/");
            return;
        }

        _ = GenerateDataAsync(idValue);
    }

    private async Task GenerateDataAsync(Guid productId)
    {
        Product = await _emporiumService.LoadDataItemAsync(productId,
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
        AllReviews = Product.Reviews;
        await RefreshAsync();

        _ = SearchReviewsAsyncCommand();
    }

    private async Task HandleSearchReviewsAsync()
    {
        await Task.Yield();

        if (string.IsNullOrWhiteSpace(SearchText))
        {
            FilteredReviews = AllReviews;
        }
        else
        {
            FilteredReviews = AllReviews
                .Where(x =>
                    x.Reviewer.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    x.Review.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                .ToArray();
        }

        TotalPages = (int)Math.Ceiling(((double)FilteredReviews.Length) / ReviewsPerPage);
        SelectedPageIndex = 1;
    }

    private async Task HandleSearchKeyPressedAsync(KeyboardEventArgs args)
    {
        if (args.Code == "Enter" || args.Code == "Return" || args.Code == "NumpadEnter")
        {
            await HandleSearchReviewsAsync();
        }

        return;
    }
}
