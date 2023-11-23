using QuickFry.ViewModels;
using CommunityToolkit.Maui.Views;

namespace QuickFry;

public partial class AccountUpdatePage : ContentPage
{
    AccountViewModel AccountVM = new AccountViewModel();

    public AccountUpdatePage()
	{
		InitializeComponent();

        txtName.Text = App.LoggedInAccout.Name;
        txtContact.Text = App.LoggedInAccout.Phone;

        SHPassword.Text = "SHOW";
        SHCPassword.Text = "SHOW";
        SHVPassword.Text = "SHOW";
        txtPassword.IsPassword = true;
        txtCPassword.IsPassword = true;
        txtVPassword.IsPassword = true;
    }

    private void PasswordChanged(object sender, TextChangedEventArgs e)
    {
        if (String.IsNullOrEmpty(txtPassword.Text))
            CPasswordPanel.IsVisible = false;
        else
        {
            txtCPassword.Text = "";
            txtCPassword.IsPassword = true;
            CPasswordPanel.IsVisible = true;
        }
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
    private void SHVerificationPasswordClicked(object sender, EventArgs e)
    {
        if (SHVPassword.Text == "SHOW")
        {
            SHVPassword.Text = "HIDE";
            txtVPassword.IsPassword = false;
        }
        else
        {
            SHVPassword.Text = "SHOW";
            txtVPassword.IsPassword = true;
        }
    }

    private async void UpdateClicked(object sender, EventArgs e)
    {
        bool b = true;
        lblNameEmpty.IsVisible = false;
        lblContactEmpty.IsVisible = false;
        lblCPasswordEmpty.IsVisible = false;
        lblPasswordsNotMatch.IsVisible = false;
        lblVPasswordEmpty.IsVisible = false;

        if (txtName.Text == App.LoggedInAccout.Name && txtContact.Text == App.LoggedInAccout.Phone && String.IsNullOrEmpty(txtPassword.Text))
        {
            await Shell.Current.DisplayAlert("Error!", "Nothing to update...", "OK");
            return;
        }

        if (String.IsNullOrWhiteSpace(txtName.Text))
        {
            if (b)
                await Shell.Current.DisplayAlert("Error!", "Missing required fields...", "OK");
            lblNameEmpty.IsVisible = true;
            b = false;
        }
        if (String.IsNullOrWhiteSpace(txtContact.Text))
        {
            if (b)
                await Shell.Current.DisplayAlert("Error!", "Missing required fields...", "OK");
            lblContactEmpty.IsVisible = true;
            b = false;
        }
        if (!String.IsNullOrWhiteSpace(txtPassword.Text))
        {
            if (txtPassword.Text.Contains(",") || txtPassword.Text.Contains("'") || txtPassword.Text.Contains("\"") || txtPassword.Text.Contains("\\") || txtPassword.Text.Contains(" "))
            {
                await Shell.Current.DisplayAlert("Error!", "Password must not contain spaces any of the following characters: comma (,), quotation mark ('), double quotation mark (\"), and backslash (\\)", "OK");
                b = false;
            }
            if (String.IsNullOrWhiteSpace(txtCPassword.Text))
            {
                if (b)
                    await Shell.Current.DisplayAlert("Error!", "Missing required fields...", "OK");
                lblCPasswordEmpty.IsVisible = true;
                b = false;
            }
            else if (txtPassword.Text != txtCPassword.Text)
            {
                await Shell.Current.DisplayAlert("Error!", "Passwords do not match...", "OK");
                lblPasswordsNotMatch.IsVisible = true;
                b = false;
            }
        }
        
        if (String.IsNullOrWhiteSpace(txtVPassword.Text))
        {
            if (b)
                await Shell.Current.DisplayAlert("Error!", "Missing required fields...", "OK");
            lblVPasswordEmpty.IsVisible = true;
            b = false;
        }

        if (b)
        {
            var updateAccount = await Shell.Current.DisplayAlert("", "Update account details?", "Yes", "No");
            if (updateAccount)
            {
                AccountVM.loadingPopup = new MauiToolkitPopupSample._0PopupLoadingPage();
                this.ShowPopup(AccountVM.loadingPopup);

                AccountVM.UpdateAccount(txtPassword.Text, txtName.Text, txtContact.Text, "", txtVPassword.Text);
            }
        }
    }
}