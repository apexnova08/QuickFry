using CommunityToolkit.Maui.Views;
using QuickFry.ViewModels;

namespace QuickFry;

public partial class AOrdersInfoClosedPage : ContentPage
{
    OrderViewModel OrderVM = new OrderViewModel();

    public AOrdersInfoClosedPage()
	{
        InitializeComponent();
        BindingContext = OrderVM;

        lblCustomerName.Text = App.SelectedOrder.UserName;
        lblUsername.Text = "@" + App.SelectedOrder.UserUsername;
        lblAddress.Text = App.SelectedOrder.Address;
        lblContact.Text = App.SelectedOrder.Contact;
        lblDate.Text = App.SelectedOrder.Date;
        lblStatus.Text = App.SelectedOrder.Status;
        lblType.Text = App.SelectedOrder.Type;
        lblID.Text = App.SelectedOrder.ID;
        lblRemarks.Text = App.SelectedOrder.Remarks;
        lblTotalCost.Text = App.SelectedOrder.TotalCost.ToString() + ".00";
        lblSubtotalCost.Text = App.SelectedOrder.SubtotalCost.ToString() + ".00";
        lblDeliveryFee.Text = App.SelectedOrder.DeliveryFee.ToString() + ".00";
        lblTotalItems.Text = App.SelectedOrder.TotalItems.ToString();
        lblSubTotalCost2.Text = App.SelectedOrder.SubtotalCost.ToString() + ".00";

        if (App.SelectedOrder.IsPaid)
            lblPaymentStatus.Text = "Paid";
        else
            lblPaymentStatus.Text = "Unpaid";

        if (String.IsNullOrEmpty(App.SelectedOrder.Remarks))
            lblRemarks.Text = "N/A";

        if (!string.IsNullOrEmpty(App.SelectedOrder.GCashTNumber))
        {
            TransactionNumberPanel.IsVisible = true;
            lblTransactionNumber.Text = App.SelectedOrder.GCashTNumber;
        }

        GetOrderItems();
    }

    async void GetOrderItems()
    {
        await OrderVM.GetOrderInfoAsync();
    }
}