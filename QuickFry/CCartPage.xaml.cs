using QuickFry.ViewModels;

namespace QuickFry;

public partial class CCartPage : ContentPage
{
	CartViewModel CartVM = new CartViewModel();

	public CCartPage()
	{
		InitializeComponent();

		BindingContext = CartVM;
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await CartVM.GetCartItemsAsync();
    }

    private async void GotoCheckoutPage(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(CCheckOutPage));
    }
}