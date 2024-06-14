using Liquidata.Emporium.Services.Interfaces;
using Liquidata.UI.Common.Pages.Common;
using Microsoft.AspNetCore.Components;

namespace Liquidata.Emporium.Pages;

public partial class HomeViewModel : ViewModelBase
{
    [Inject] private IEmporiumService _emporiumService { get; set; } = null!;

    public bool IsGeneratingData { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        IsGeneratingData = true;
        await RefreshAsync();

        _ = Task.Run(async () =>
        {            
            await _emporiumService.GenerateDataAsync();
            IsGeneratingData = false;
            await RefreshAsync();
        });
    }
}
