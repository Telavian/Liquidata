using Liquidata.Emporium.Models;
using Liquidata.UI.Common.Pages.Common;
using Microsoft.AspNetCore.Components;

namespace Liquidata.Emporium.Pages;

public class CategoryListViewModel : ViewModelBase
{
    [Parameter] public EmporiumData Data { get; set; }
    
    public EmporiumCategory SelectedCategory { get; set; } = null!;
}
