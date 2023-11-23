using QuickFry.ViewModels;

namespace QuickFry;

public partial class ASCategoriesPage : ContentPage
{
    CategoryViewModel CategoryVM = new CategoryViewModel();

    public ASCategoriesPage()
	{
		InitializeComponent();

        BindingContext = CategoryVM;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        GetCtgs();
    }

    async void GetCtgs()
    {
        await CategoryVM.GetCategoriesAsync();
    }

    private async void GotoAdd(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(ASCtgAddPage));
    }
}