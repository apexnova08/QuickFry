namespace QuickFry;

public partial class LoginSelectPage : ContentPage
{
    public string TestString { get; set; }
	public LoginSelectPage()
	{
		InitializeComponent();

        TestString = File.ReadAllText(App.LoggedInAccountPath);

        BindingContext = this;
	}

    private async void AdminLogin(object sender, EventArgs e)
    {
        if (!App.CheckInternetConnection())
        {
            await DisplayAlert("Error", "No internet connection.", "OK");
            return;
        }

        await Shell.Current.GoToAsync("//ADashboardPage");
    }
    private async void CustomerLogin(object sender, EventArgs e)
    {
        if (!App.CheckInternetConnection())
        {
            await DisplayAlert("Error", "No internet connection.", "OK");
            return;
        }

        await Shell.Current.GoToAsync("//CMenuPage");
    }
    private async void RiderLogin(object sender, EventArgs e)
    {
        if (!App.CheckInternetConnection())
        {
            await DisplayAlert("Error", "No internet connection.", "OK");
            return;
        }

        await Shell.Current.GoToAsync("//RiderPage");
    }
    private async void Logout(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//LoginPage");
    }
}