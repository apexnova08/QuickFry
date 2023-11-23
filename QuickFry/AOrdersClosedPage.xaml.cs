using QuickFry.ViewModels;

namespace QuickFry;

public partial class AOrdersClosedPage : ContentPage
{
    OrderViewModel OrderVM = new OrderViewModel();

    public AOrdersClosedPage()
	{
		InitializeComponent();

        BindingContext = OrderVM;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await OrderVM.GetOrdersClosedAsync();
    }
}