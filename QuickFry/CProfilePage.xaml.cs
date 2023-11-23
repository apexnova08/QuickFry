using QuickFry.ViewModels;

namespace QuickFry;

public partial class CProfilePage : ContentPage
{
    AccountViewModel AccountVM = new AccountViewModel();

	public CProfilePage()
	{
		InitializeComponent();
	}
    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (App.LoggedInAccout == null)
        {
            lblName.Text = "Guest";
            lblUsername.Text = "";
            lblLoggedOut.IsVisible = true;

            LoggedOutPanel.IsVisible = true;
            LoggedInPanel.IsVisible = false;
        }
        else
        {
            AccountVM.UpdateAccountData();

            lblName.Text = App.LoggedInAccout.Name;
            lblUsername.Text = "@" + App.LoggedInAccout.Username;
            lblLoggedOut.IsVisible = false;

            LoggedOutPanel.IsVisible = false;
            LoggedInPanel.IsVisible = true;
        }
    }

    private async void GotoLogInPage(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//LoginPage");
    }
    private async void GotoRegisterPage(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(CRegisterPage));
    }
    private async void GotoAccountUpdatePage(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(AccountUpdatePage));
    }
    private async void GotoAddressesPage(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(CPAddressesPage));
    }
}