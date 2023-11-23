using QuickFry.ViewModels;

namespace QuickFry;

public partial class AOrdersFinishedPage : ContentPage
{
    OrderViewModel OrderVM = new OrderViewModel();

    public AOrdersFinishedPage()
	{
		InitializeComponent();

        BindingContext = OrderVM;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await OrderVM.GetOrdersFinishedAsync();
    }
}