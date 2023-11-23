using QuickFry.ViewModels;

namespace QuickFry;

public partial class InitialPage : ContentPage
{
    AccountViewModel AccountVM = new AccountViewModel();

    public InitialPage()
	{
		InitializeComponent();
        BindingContext = AccountVM;

        AccountVM.CheckLogin();
    }
}