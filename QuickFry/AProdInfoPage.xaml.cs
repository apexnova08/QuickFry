using QuickFry.ViewModels;

namespace QuickFry;

public partial class AProdInfoPage : ContentPage
{
    ProductViewModel ProdVM = new ProductViewModel();

    public AProdInfoPage()
	{
        InitializeComponent();

        lblID.Text = App.SelectedProductDisplay.ID;
        imgProd.Source = App.SelectedProductDisplay.Image;
        lblName.Text = App.SelectedProductDisplay.Name;
        lblCategory.Text = App.SelectedProductDisplay.CategoryName;
        lblCost.Text = App.SelectedProductDisplay.Cost.ToString() + ".00";
        lblDescription.Text = App.SelectedProductDisplay.Description;

        if (App.SelectedProduct.Archived)
            btnDisable.Text = "Enable";
        else
        {
            btnDisable.Text = "Disable";
            btnDisable.BackgroundColor = Colors.Red;
        }
            
    }

    private async void GotoEditPage(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("../" + nameof(AProdEditPage));
    }
    private async void DisableClicked(object sender, EventArgs e)
    {
        if (App.SelectedProductDisplay.Archived)
        {
            var enable = await Shell.Current.DisplayAlert("", "Enable product?", "Yes", "No");
            if (enable)
                ProdVM.DisableProduct(false);
        }
        else
        {
            var disable = await Shell.Current.DisplayAlert("", "Disable product?", "Yes", "No");
            if (disable)
                ProdVM.DisableProduct(true);
        }
    }
}