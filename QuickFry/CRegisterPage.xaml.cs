using QuickFry.ViewModels;
using CommunityToolkit.Maui.Views;

namespace QuickFry;

public partial class CRegisterPage : ContentPage
{
    AccountViewModel AccVM = new AccountViewModel();

    public CRegisterPage()
	{
		InitializeComponent();

        SHPassword.Text = "SHOW";
        SHCPassword.Text = "SHOW";
        txtPassword.IsPassword = true;
        txtCPassword.IsPassword = true;
    }

    private void SHPasswordClicked(object sender, EventArgs e)
    {
        if (SHPassword.Text == "SHOW")
        {
            SHPassword.Text = "HIDE";
            txtPassword.IsPassword = false;
        }
        else
        {
            SHPassword.Text = "SHOW";
            txtPassword.IsPassword = true;
        }
    }
    private void SHConfirmPasswordClicked(object sender, EventArgs e)
    {
        if (SHCPassword.Text == "SHOW")
        {
            SHCPassword.Text = "HIDE";
            txtCPassword.IsPassword = false;
        }
        else
        {
            SHCPassword.Text = "SHOW";
            txtCPassword.IsPassword = true;
        }
    }
    private async void RegisterClicked(object sender, EventArgs e)
    {
        bool b = true;
        lblNameEmpty.IsVisible = false;
        lblAddressEmpty.IsVisible = false;
        lblContactEmpty.IsVisible = false;
        lblUserEmpty.IsVisible = false;
        lblPasswordEmpty.IsVisible = false;
        lblCPasswordEmpty.IsVisible = false;
        lblPasswordsNotMatch.IsVisible = false;

        if (String.IsNullOrWhiteSpace(txtName.Text))
        {
            if (b)
                await Shell.Current.DisplayAlert("Error!", "Missing required fields...", "OK");
            lblNameEmpty.IsVisible = true;
            b = false;
        }
        if (String.IsNullOrWhiteSpace(txtAddress.Text))
        {
            if (b)
                await Shell.Current.DisplayAlert("Error!", "Missing required fields...", "OK");
            lblAddressEmpty.IsVisible = true;
            b = false;
        }
        if (String.IsNullOrWhiteSpace(txtContact.Text))
        {
            if (b)
                await Shell.Current.DisplayAlert("Error!", "Missing required fields...", "OK");
            lblContactEmpty.IsVisible = true;
            b = false;
        }
        if (String.IsNullOrWhiteSpace(txtUser.Text))
        {
            if (b)
                await Shell.Current.DisplayAlert("Error!", "Missing required fields...", "OK");
            lblUserEmpty.IsVisible = true;
            b = false;
        }
        else if (txtUser.Text.Contains(",") || txtUser.Text.Contains("'") || txtUser.Text.Contains("\"") || txtUser.Text.Contains("\\") || txtUser.Text.Contains(" "))
        {
            await Shell.Current.DisplayAlert("Error!", "Username must not contain spaces or any of the following characters: comma (,), quotation mark ('), double quotation mark (\"), and backslash (\\)", "OK");
            b = false;
        }
        if (String.IsNullOrWhiteSpace(txtPassword.Text))
        {
            if (b)
                await Shell.Current.DisplayAlert("Error!", "Missing required fields...", "OK");
            lblPasswordEmpty.IsVisible = true;
            b = false;
        }
        else if (txtPassword.Text.Contains(",") || txtPassword.Text.Contains("'") || txtPassword.Text.Contains("\"") || txtPassword.Text.Contains("\\") || txtPassword.Text.Contains(" "))
        {
            await Shell.Current.DisplayAlert("Error!", "Password must not contain spaces or any of the following characters: comma (,), quotation mark ('), double quotation mark (\"), and backslash (\\)", "OK");
            b = false;
        }
        if (String.IsNullOrWhiteSpace(txtCPassword.Text))
        {
            if (b)
                await Shell.Current.DisplayAlert("Error!", "Missing required fields...", "OK");
            lblCPasswordEmpty.IsVisible = true;
            b = false;
        }
        if (!String.IsNullOrWhiteSpace(txtPassword.Text) && !String.IsNullOrWhiteSpace(txtCPassword.Text) && txtPassword.Text != txtCPassword.Text)
        {
            await Shell.Current.DisplayAlert("Error!", "Passwords do not match...", "OK");
            lblPasswordsNotMatch.IsVisible = true;
            b = false;
        }

        if (b)
        {
            var addAccount = await Shell.Current.DisplayAlert("", "Create new account?", "Yes", "No");
            if (addAccount)
            {
                AccVM.loadingPopup = new MauiToolkitPopupSample._0PopupLoadingPage();
                this.ShowPopup(AccVM.loadingPopup);

                AccVM.RegisterAccount(txtUser.Text, txtPassword.Text, txtName.Text, txtAddress.Text, txtContact.Text, "");
            }
        }
    }
}