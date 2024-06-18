using Liquidata.Client.Emporium.Models;
using Liquidata.UI.Common.Pages.Common;
using Microsoft.AspNetCore.Components;

namespace Liquidata.Client.Emporium.Pages;

public partial class EmporiumCategoryListViewModel : ViewModelBase
{
    [Parameter] public EmporiumData Data { get; set; } = null!;

    public EmporiumCategory SelectedCategory { get; set; } = null!;
}
