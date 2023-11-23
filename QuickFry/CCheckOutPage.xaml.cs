using CommunityToolkit.Maui.Views;
using QuickFry.ViewModels;

namespace QuickFry;

public partial class CCheckOutPage : ContentPage
{
    public static CartViewModel CartVM = new CartViewModel();

    public CCheckOutPage()
	{
		InitializeComponent();

		BindingContext = CartVM;

        App.SelectedAddress = App.DefaultAddress;

        lblAddressName.Text = App.SelectedAddress.Name;
        lblAddress.Text = App.SelectedAddress.Address;

        txtContact.Text = App.LoggedInAccout.Phone;

        Getitems();
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();

        lblAddressName.Text = App.SelectedAddress.Name;
        lblAddress.Text = App.SelectedAddress.Address;
    }

    async void Getitems()
    {
        await CartVM.GetCartItemsAsync();
    }

    private async void GotoPickAddressPage(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(CCPickAddressPage));
    }
    private void AcquisitionTypeChanged(object sender, CheckedChangedEventArgs e)
    {
        if (rbDelivery.IsChecked)
        {
            CartVM.GrandTotal = CartVM.TotalCost + CartVM.DeliveryFee;
            deliveryCB.IsVisible = true;
            deliveryBreakdown.IsVisible = true;

            rbCash.Content = "Cash on Delivery";
        }
            
        else
        {
            CartVM.GrandTotal = CartVM.TotalCost;
            deliveryCB.IsVisible = false;
            deliveryBreakdown.IsVisible = false;

            rbCash.Content = "Cash on Pickup";
        }
    }

    private async void OrderClicked(object sender, EventArgs e)
    {
        bool b = true;

        if (String.IsNullOrWhiteSpace(txtContact.Text))
        {
            if (b)
                await Shell.Current.DisplayAlert("Error!", "Missing required fields...", "OK");
            b = false;
        }
        if (!rbDelivery.IsChecked && !rbPickup.IsChecked)
        {
            if (b)
                await Shell.Current.DisplayAlert("Error!", "Please select your desired form of acquisition...", "OK");
            b = false;
        }
        if (!rbCash.IsChecked && !rbGCash.IsChecked)
        {
            if (b)
                await Shell.Current.DisplayAlert("Error!", "Please select your desired method of payment...", "OK");
            b = false;
        }

        if (b)
        {
            string acquisition;
            string paymentMethod;

            if (rbDelivery.IsChecked)
                acquisition = "Delivery";
            else
                acquisition = "Pickup";

            if (rbCash.IsChecked)
                paymentMethod = "Cash";
            else
                paymentMethod = "GCash";

            var order = await Shell.Current.DisplayAlert("", "Submit order?", "Yes", "No");
            if (order)
            {
                CartVM.loadingPopup = new MauiToolkitPopupSample._0PopupLoadingPage();
                this.ShowPopup(CartVM.loadingPopup);

                CartVM.PlaceOrder(lblAddress.Text, txtContact.Text, acquisition, paymentMethod, txtRemarks.Text);
            }
        }
    }
}