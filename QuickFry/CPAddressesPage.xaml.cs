using QuickFry.ViewModels;

namespace QuickFry;

public partial class CPAddressesPage : ContentPage
{
	AccountViewModel AccountVM = new AccountViewModel();

	public CPAddressesPage()
	{
		InitializeComponent();

		BindingContext = AccountVM;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();

        GetAddresses();
    }

    async void GetAddresses()
    {
        await AccountVM.GetAddressesAsync();
    }

    private async void GotoAddAddressPage(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(CPAddressAddPage));
    }
}