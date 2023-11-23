using CommunityToolkit.Maui.Views;
using QuickFry.ViewModels;

namespace QuickFry;

public partial class CPAddressEditPage : ContentPage
{
    AccountViewModel AccountVM = new AccountViewModel();

	public CPAddressEditPage()
	{
		InitializeComponent();

        txtName.Text = App.SelectedAddress.Name;
        txtAddress.Text = App.SelectedAddress.Address;
        chkDefault.IsChecked = App.SelectedAddress.IsDefault;
	}

    private async void UpdateClicked(object sender, EventArgs e)
    {
        if (txtName.Text == App.SelectedAddress.Name && txtAddress.Text == App.SelectedAddress.Address && chkDefault.IsChecked == App.SelectedAddress.IsDefault)
        {
            await Shell.Current.DisplayAlert("Error!", "Nothing to update...", "OK");
            return;
        }
        if (String.IsNullOrEmpty(txtName.Text) || String.IsNullOrEmpty(txtAddress.Text))
        {
            await Shell.Current.DisplayAlert("Error!", "Missing required fields...", "OK");
            return;
        }
        var addAddress = await Shell.Current.DisplayAlert("", "Update address?", "Yes", "No");
        if (addAddress)
        {
            AccountVM.loadingPopup = new MauiToolkitPopupSample._0PopupLoadingPage();
            this.ShowPopup(AccountVM.loadingPopup);

            AccountVM.UpdateAddress(txtName.Text, txtAddress.Text, chkDefault.IsChecked);
        }
    }
}