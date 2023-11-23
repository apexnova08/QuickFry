using CommunityToolkit.Maui.Views;
using QuickFry.ViewModels;
using System.Xml.Linq;

namespace QuickFry;

public partial class ASCtgEditPage : ContentPage
{
    CategoryViewModel CategoryVM = new CategoryViewModel();

    public ASCtgEditPage()
	{
		InitializeComponent();

        txtCtgName.Text = App.SelectedCategory.Name;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
    }

    private async void EditClicked(Object sender, EventArgs e)
    {
        if (txtCtgName.Text == App.SelectedCategory.Name)
        {
            await Shell.Current.DisplayAlert("Error!", "Nothing to update...", "OK");
            return;
        }
        if (String.IsNullOrEmpty(txtCtgName.Text))
        {
            await Shell.Current.DisplayAlert("Error!", "Missing required fields...", "OK");
            return;
        }

        var editCategory = await Shell.Current.DisplayAlert("", "Update category?", "Yes", "No");
        if (editCategory)
        {
            CategoryVM.loadingPopup = new MauiToolkitPopupSample._0PopupLoadingPage();
            this.ShowPopup(CategoryVM.loadingPopup);

            CategoryVM.EditCategory(txtCtgName.Text);
        }
    }
}