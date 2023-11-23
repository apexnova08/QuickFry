using CommunityToolkit.Maui.Views;
using QuickFry.ViewModels;

namespace QuickFry;

public partial class COrderInfoPage : ContentPage
{
    OrderViewModel OrderVM = new OrderViewModel();

    public COrderInfoPage()
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

        if (String.IsNullOrEmpty(App.SelectedOrder.Remarks))
            lblRemarks.Text = "N/A";

        if (App.SelectedOrder.Status == "Pending")
        {
            btnPayGCash.IsEnabled = false;
            btnCancel.IsVisible = true;
        }
        if (App.SelectedOrder.IsPaid)
        {
            btnPayGCash.IsEnabled = false;
            lblPaymentStatus.Text = "Paid";
        }  
        else
            lblPaymentStatus.Text = "Unpaid";

        if (!string.IsNullOrEmpty(App.SelectedOrder.GCashTNumber))
        {
            btnPayGCash.IsVisible = false;
            TransactionNumberPanel.IsVisible = true;
            lblTransactionNumber.Text = App.SelectedOrder.GCashTNumber;
        }

        GetOrderItems();
    }

    async void GetOrderItems()
    {
        await OrderVM.GetOrderInfoAsync();
    }

    private async void PayGCashClicked(object sender, EventArgs e)
    {
        if (btnPayGCash.Text == "Pay with GCash")
        {
            PayGCashPanel.IsVisible = true;
            btnPayGCash.Text = "Save";
            btnCancelPayGCash.IsVisible = true;
        }
        else if (btnPayGCash.Text == "Save")
        {
            if (string.IsNullOrEmpty(txtTransactionNumber.Text))
            {
                await Shell.Current.DisplayAlert("Error!", "Field is empty.", "OK");
                return;
            }

            OrderVM.loadingPopup = new MauiToolkitPopupSample._0PopupLoadingPage();
            this.ShowPopup(OrderVM.loadingPopup);

            OrderVM.UpdateOrderGCashTransactionNumber(txtTransactionNumber.Text);
        }
    }
    private void CancelPayGCashClicked(object sender, EventArgs e)
    {
        txtTransactionNumber.Text = "";
        PayGCashPanel.IsVisible = false;
        btnPayGCash.Text = "Pay with GCash";
        btnCancelPayGCash.IsVisible = false;
    }

    private async void PasteTransactionNumber(object sender, EventArgs e)
    {
        if (Clipboard.Default.HasText)
            txtTransactionNumber.Text = await Clipboard.Default.GetTextAsync();
    }

    private async void CancelOrderClicked(object sender, EventArgs e)
    {
        var cancel = await Shell.Current.DisplayAlert("", "This order will be cancelled, continue?", "Yes", "No");
        if (cancel)
        {
            OrderVM.loadingPopup = new MauiToolkitPopupSample._0PopupLoadingPage();
            this.ShowPopup(OrderVM.loadingPopup);

            OrderVM.CancelOrder();
        }
    }

    private async void CopyGCashRefNumber(object sender, TappedEventArgs e) =>
        await Clipboard.Default.SetTextAsync(lblGCashNumber.Text);
}