namespace Liquidata.Client.Emporium.Models;

public class EmporiumFeaturedCategory
{
    public EmporiumCategory Category { get; set; } = null!;
    public EmporiumItem[] Items { get; set; } = [];
}
