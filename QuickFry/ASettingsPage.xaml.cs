namespace QuickFry;

public partial class ASettingsPage : ContentPage
{
	public ASettingsPage()
	{
		InitializeComponent();
	}

    private async void GotoAccounts(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(ASAccountsPage));
    }
    private async void GotoCategories(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(ASCategoriesPage));
    }

    private async void GotoDeliveryFee(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(ASDeliveryFeePage));
    }

    private async void GotoGCashQR(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(ASGCashQRPage));
    }
}