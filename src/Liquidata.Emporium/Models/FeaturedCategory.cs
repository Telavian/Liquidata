namespace Liquidata.Emporium.Models
{
    public class FeaturedCategory
    {
        public EmporiumCategory Category { get; set; } = null!;
        public EmporiumItem[] Items { get; set; } = [];
    }
}
