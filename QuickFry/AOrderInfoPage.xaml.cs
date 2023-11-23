using CommunityToolkit.Maui.Views;
using QuickFry.ViewModels;

namespace QuickFry;

public partial class AOrderInfoPage : ContentPage
{
	OrderViewModel OrderVM = new OrderViewModel();

	public AOrderInfoPage()
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
        {
            lblPaymentStatus.Text = "Unpaid";
            btnUpdate.IsVisible = true;
            btnPaid.IsVisible = true;
        }

        if (String.IsNullOrEmpty(App.SelectedOrder.Remarks))
            lblRemarks.Text = "N/A";

        if (!string.IsNullOrEmpty(App.SelectedOrder.GCashTNumber))
        {
            TransactionNumberPanel.IsVisible = true;
            lblTransactionNumber.Text = App.SelectedOrder.GCashTNumber;
        }

        if (App.SelectedOrder.ToPay <= 0)
            btnPaid.IsEnabled = true;
        else
            btnPaid.IsEnabled = false;

        if (App.SelectedOrder.Type == "Delivery")
            btnPrepared.Text = "Mark order for delivery";
        if (App.SelectedOrder.Type == "Pickup")
            btnPrepared.Text = "Mark order for pickup";

        if (App.SelectedOrder.Status == "Pending")
        {
            btnAccept.IsVisible = true;
            btnReject.IsVisible = true;

            btnUpdate.IsVisible = false;
            btnPaid.IsVisible = false;
        }
        if (App.SelectedOrder.Status == "Preparing")
        {
            btnPrepared.IsVisible = true;
        }
        if (App.SelectedOrder.Status == "Out for Delivery")
        {
            btnClose.IsVisible = true;
            btnClose.Text = "Mark as Delivered";
        }
        if (App.SelectedOrder.Status == "Ready for Pickup")
        {
            btnClose.IsVisible = true;
            btnClose.Text = "Mark as Picked Up";
        }
        if (!App.SelectedOrder.IsPaid)
            btnClose.IsEnabled = false;

        GetOrderItems();
    }

    async void GetOrderItems()
    {
        await OrderVM.GetOrderInfoAsync();
    }

    private async void UpdatePaymentClicked(object sender, EventArgs e)
    {
        if (btnUpdate.Text == "Update Payment")
        {
            UpdatePaymentPanel.IsVisible = true;
            btnUpdate.Text = "Save";
            btnCancelUpdate.IsVisible = true;

            txtPaidCash.Text = OrderVM.PaidCash.ToString();
            txtPaidGCash.Text = OrderVM.PaidGCash.ToString();
        }
        else if (btnUpdate.Text == "Save")
        {
            if (string.IsNullOrEmpty(txtPaidCash.Text) || string.IsNullOrEmpty(txtPaidGCash.Text))
            {
                await Shell.Current.DisplayAlert("Error!", "Missing required fields.", "OK");
                return;
            }
            if (int.TryParse(txtPaidCash.Text, out int cashnum) && int.TryParse(txtPaidGCash.Text, out int gcashnum))
            {
                if (OrderVM.PaidCash == cashnum && OrderVM.PaidGCash == gcashnum)
                {
                    await Shell.Current.DisplayAlert("Error!", "Nothing to update.", "OK");
                    return;
                }
                OrderVM.loadingPopup = new MauiToolkitPopupSample._0PopupLoadingPage();
                this.ShowPopup(OrderVM.loadingPopup);

                await OrderVM.UpdateOrderPayment(cashnum, gcashnum);

                UpdatePaymentPanel.IsVisible = false;
                btnUpdate.Text = "Update Payment";
                btnCancelUpdate.IsVisible = false;

                if (OrderVM.ToPay > 0)
                    btnPaid.IsEnabled = false;
                else
                    btnPaid.IsEnabled = true;
            }
            else
            {
                await Shell.Current.DisplayAlert("Error!", "Invalid input.", "OK");
                return;
            }
        }
    }
    private void CancelUpdateClicked(object sender, EventArgs e)
    {
        txtPaidCash.Text = "";
        txtPaidGCash.Text = "";
        UpdatePaymentPanel.IsVisible = false;
        btnUpdate.Text = "Update Payment";
        btnCancelUpdate.IsVisible = false;
    }

    private void AcceptOrderClicked(object sender, EventArgs e)
    {
        OrderVM.loadingPopup = new MauiToolkitPopupSample._0PopupLoadingPage();
        this.ShowPopup(OrderVM.loadingPopup);

        OrderVM.AcceptOrder();
    }

    private async void PreparedClicked(object sender, EventArgs e)
    {
        var prepared = await Shell.Current.DisplayAlert("", "Mark order for " + App.SelectedOrder.Type + "?", "Yes", "No");
        if (prepared)
        {
            OrderVM.loadingPopup = new MauiToolkitPopupSample._0PopupLoadingPage();
            this.ShowPopup(OrderVM.loadingPopup);

            OrderVM.OrderPrepared();
        }
    }

    private async void PaidClicked(object sender, EventArgs e)
    {
        var paid = await Shell.Current.DisplayAlert("", "Mark order as fully paid?", "Yes", "No");
        if (paid)
        {
            OrderVM.loadingPopup = new MauiToolkitPopupSample._0PopupLoadingPage();
            this.ShowPopup(OrderVM.loadingPopup);

            OrderVM.MarkAsPaid();
        }
    }

    private async void CloseClicked(object sender, EventArgs e)
    {
        var close = await Shell.Current.DisplayAlert("", "This order will be closed, continue?", "Yes", "No");
        if (close)
        {
            OrderVM.loadingPopup = new MauiToolkitPopupSample._0PopupLoadingPage();
            this.ShowPopup(OrderVM.loadingPopup);

            OrderVM.OrderDone();
        }
    }

    private async void RejectOrderClicked(object sender, EventArgs e)
    {
        var cancel = await Shell.Current.DisplayAlert("", "This order will be rejected, continue?", "Yes", "No");
        if (cancel)
        {
            OrderVM.loadingPopup = new MauiToolkitPopupSample._0PopupLoadingPage();
            this.ShowPopup(OrderVM.loadingPopup);

            OrderVM.RejectOrder();
        }
    }
    private async void CopyContact(object sender, TappedEventArgs e) =>
        await Clipboard.Default.SetTextAsync(lblContact.Text);

    private async void CopyGCashTNum(object sender, TappedEventArgs e) =>
        await Clipboard.Default.SetTextAsync(lblTransactionNumber.Text);
}