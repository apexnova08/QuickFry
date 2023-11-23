using QuickFry.ViewModels;

namespace QuickFry;

public partial class RiderPage : ContentPage
{
    OrderViewModel OrderVM = new OrderViewModel();

    public RiderPage()
	{
		InitializeComponent();

        BindingContext = OrderVM;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await OrderVM.GetOrdersRiderAsync();
    }

    private async void LogoutClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//LoginPage");
    }
}