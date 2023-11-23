using QuickFry.ViewModels;

namespace QuickFry;

public partial class ASAccountsPage : ContentPage
{
    AccountViewModel AccountVM = new AccountViewModel();

    public ASAccountsPage()
	{
		InitializeComponent();

		BindingContext = AccountVM;
	}

    protected async override void OnAppearing()
    {
        base.OnAppearing();

        await AccountVM.GetAccountsAsync();
    }

    private async void AddAccountClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(ASAccountAddPage));
    }

    private async void EditAccountClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(AccountUpdatePage));
    }
}