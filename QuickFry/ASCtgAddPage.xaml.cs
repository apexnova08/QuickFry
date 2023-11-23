using CommunityToolkit.Maui.Views;
using QuickFry.ViewModels;

namespace QuickFry;

public partial class ASCtgAddPage : ContentPage
{
    CategoryViewModel CategoryVM = new CategoryViewModel();

    public ASCtgAddPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
    }

    private async void AddClicked(Object sender, EventArgs e)
    {
        if (String.IsNullOrEmpty(txtCtgName.Text))
        {
            await Shell.Current.DisplayAlert("Error!", "Missing required fields...", "OK");
            return;
        }

        var addCategory = await Shell.Current.DisplayAlert("", "Add new category?", "Yes", "No");
        if (addCategory)
        {
            CategoryVM.loadingPopup = new MauiToolkitPopupSample._0PopupLoadingPage();
            this.ShowPopup(CategoryVM.loadingPopup);

            CategoryVM.AddCategory(txtCtgName.Text);
        }
    }
}