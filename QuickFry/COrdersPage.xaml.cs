using QuickFry.ViewModels;

namespace QuickFry;

public partial class COrdersPage : ContentPage
{
	OrderViewModel OrderVM = new OrderViewModel();

	public COrdersPage()
	{
		InitializeComponent();

		BindingContext = OrderVM;
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();

		await OrderVM.GetOrdersUserAsync();
    }
}