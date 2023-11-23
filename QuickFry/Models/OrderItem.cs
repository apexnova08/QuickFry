namespace QuickFry.Models;

public class OrderItem
{
    public string ID { get; set; }
    public string OrderID { get; set; }
    public string Item { get; set; }
    public int Cost { get; set; }
    public int Amount { get; set; }
    public int SubTotal { get; set; }
}
