namespace QuickFry.Models;

public class Order
{
    public string ID { get; set; }
    public string Date { get; set; }
    public string User { get; set; }
    public string Address { get; set; }
    public string Contact { get; set; }
    public string Type { get; set; }
    public int DeliveryFee { get; set; }
    public int TotalItems { get; set; }
    public int SubtotalCost { get; set; }
    public int TotalCost { get; set; }
    public string PaymentMethod { get; set; }
    public int PaidCash { get; set; }
    public int PaidGCash { get; set; }
    public int Change { get; set; }
    public string GCashTNumber { get; set; }
    public int ToPay { get; set; }
    public bool IsPaid { get; set; }
    public string Remarks { get; set; }
    public string Status { get; set; }
    public string ReasonForClosing { get; set; }
    public bool IsClosed { get; set; }
}
