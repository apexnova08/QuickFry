namespace QuickFry.Models;

public class Product
{
    public string ID { get; set; }
    public string Name { get; set; }
    public int Cost { get; set; }
    public string Image { get; set; }
    public string Description { get; set; }
    public string Category { get; set; }
    public bool Archived { get; set; }
}
