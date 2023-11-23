using CommunityToolkit.Maui.Views;
using QuickFry.ViewModels;

namespace QuickFry;

public partial class LoginPage : ContentPage
{
    AccountViewModel AccountVM = new AccountViewModel();

    public LoginPage()
	{
		InitializeComponent();
	}
    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (App.LoggedInAccout != null)
            txtUser.Text = App.LoggedInAccout.Username;
        txtPass.Text = "";

        App.ClearLogInInfo();
    }

    private void logIn(object sender, EventArgs e)
    {
        AccountVM.loadingPopup = new MauiToolkitPopupSample._0PopupLoadingPage();
        this.ShowPopup(AccountVM.loadingPopup);

        AccountVM.LogIn(txtUser.Text, txtPass.Text);
    }
    private async void ContinueAsGuest(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//CMenuPage");
    }
}