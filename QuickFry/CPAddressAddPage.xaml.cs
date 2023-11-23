using CommunityToolkit.Maui.Views;
using QuickFry.ViewModels;

namespace QuickFry;

public partial class CPAddressAddPage : ContentPage
{
	AccountViewModel AccountVM = new AccountViewModel();

	public CPAddressAddPage()
	{
		InitializeComponent();
	}

    private async void AddClicked(object sender, EventArgs e)
    {
		if (String.IsNullOrEmpty(txtName.Text) || String.IsNullOrEmpty(txtAddress.Text))
		{
            await Shell.Current.DisplayAlert("Error!", "Missing required fields...", "OK");
            return;
		}
        var addAddress = await Shell.Current.DisplayAlert("", "Create new account?", "Yes", "No");
        if (addAddress)
        {
            AccountVM.loadingPopup = new MauiToolkitPopupSample._0PopupLoadingPage();
            this.ShowPopup(AccountVM.loadingPopup);

            AccountVM.AddAddress(txtName.Text, txtAddress.Text);
        }
    }
}