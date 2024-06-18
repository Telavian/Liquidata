namespace Liquidata.Client.Emporium.Models;

public class EmporiumItem
{
    public Guid ProductId { get; set; } = Guid.NewGuid();
    public string ImageLink { get; set; } = "";
    public float StarRating { get; set; }
    public string Manufacturer { get; set; } = "";
    public string Name { get; set; } = "";
    public float Price { get; set; }
    public int Quantity { get; set; }
    public EmporiumCategory Category { get; set; } = null!;
    public string Description { get; set; } = "";
    public EmporiumReview[] Reviews { get; set; } = [];
    public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();
}
