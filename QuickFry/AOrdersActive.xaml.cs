using QuickFry.ViewModels;

namespace QuickFry;

public partial class AOrdersActive : ContentPage
{
    OrderViewModel OrderVM = new OrderViewModel();

    public AOrdersActive()
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