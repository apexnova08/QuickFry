using QuickFry.ViewModels;

namespace QuickFry;

public partial class AOrdersActivePage : ContentPage
{
    OrderViewModel OrderVM = new OrderViewModel();

    public AOrdersActivePage()
	{
		InitializeComponent();

        BindingContext = OrderVM;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await OrderVM.GetOrdersActiveAsync();
    }
}