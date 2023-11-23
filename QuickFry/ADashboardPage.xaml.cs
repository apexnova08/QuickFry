using QuickFry.ViewModels;

namespace QuickFry;


public partial class ADashboardPage : ContentPage
{
    OrderViewModel OrderVM = new OrderViewModel();

    public ADashboardPage()
	{
		InitializeComponent();

        BindingContext = OrderVM;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await OrderVM.GetOrdersDashboardAsync();
    }
}