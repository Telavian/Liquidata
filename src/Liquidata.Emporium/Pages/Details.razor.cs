using Liquidata.UI.Common.Pages.Common;
using Microsoft.AspNetCore.Components;

namespace Liquidata.Emporium.Pages;

public class DetailsViewModel : ViewModelBase
{
    [Parameter]
    [SupplyParameterFromQuery]
    public Guid ProductId { get; set; }
}
