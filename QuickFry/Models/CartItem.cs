namespace QuickFry.Models;

public class CartItem
{
    public string ID { get; set; }
    public string Item { get; set; }
    public string User { get; set; }
    public int Amount { get; set; }
    public int SubTotal { get; set; }
}
