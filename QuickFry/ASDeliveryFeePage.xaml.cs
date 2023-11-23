using CommunityToolkit.Maui.Views;
using QuickFry.ViewModels;

namespace QuickFry;

public partial class ASDeliveryFeePage : ContentPage
{
	SystemViewModel SystemVM = new SystemViewModel();

	public ASDeliveryFeePage()
	{
		InitializeComponent();

        BindingContext = this;

		GetDeliveryFee();
    }

	async void GetDeliveryFee()
	{
        await SystemVM.GetDeliveryFeeAsync();

        txtDeliveryFee.Text = SystemVM.DeliveryFee.ToString();
    }

    private async void UpdateClicked(Object sender, EventArgs e)
    {
        if (txtDeliveryFee.Text == SystemVM.DeliveryFee.ToString())
        {
            await Shell.Current.DisplayAlert("Error!", "Nothing to update...", "OK");
            return;
        }
        if (String.IsNullOrEmpty(txtDeliveryFee.Text))
        {
            await Shell.Current.DisplayAlert("Error!", "Missing required fields...", "OK");
            return;
        }

        if (int.TryParse(txtDeliveryFee.Text, out int num))
        {
            var editdeliveryfee = await Shell.Current.DisplayAlert("", "Update delivery fee?", "Yes", "No");
            if (editdeliveryfee)
            {
                SystemVM.loadingPopup = new MauiToolkitPopupSample._0PopupLoadingPage();
                this.ShowPopup(SystemVM.loadingPopup);

                SystemVM.EditDeliveryFee(num);
            }
        }
        else
        {
            await Shell.Current.DisplayAlert("Error!", "Please enter a proper value...", "OK");
            return;
        }
    }
}