using QuickFry.ViewModels;

namespace QuickFry;

public partial class CCPickAddressPage : ContentPage
{
    AccountViewModel AccountVM = new AccountViewModel();

    public CCPickAddressPage()
	{
		InitializeComponent();

        BindingContext = AccountVM;

        GetAddresses();
    }

    async void GetAddresses()
    {
        await AccountVM.GetAddressesAsync();
    }
}