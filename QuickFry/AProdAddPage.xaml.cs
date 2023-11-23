using CommunityToolkit.Maui.Views;
using QuickFry.ViewModels;

namespace QuickFry;

public partial class AProdAddPage : ContentPage
{
    ProductViewModel ProdVM = new ProductViewModel();

    Stream imgStream;

    public AProdAddPage()
	{
		InitializeComponent();

        BindingContext = ProdVM;
        ProdVM.GetCategories();
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
    }

    private async void AddClicked(Object sender, EventArgs e)
    {
        if (!App.CheckInternetConnection())
        {
            await DisplayAlert("Error", "No internet connection.", "OK");
            return;
        }

        bool b = true;
        lblNameEmpty.IsVisible = false;
        lblImageEmpty.IsVisible = false;
        lblCategoryEmpty.IsVisible = false;
        lblCostEmpty.IsVisible = false;
        lblDescriptionEmpty.IsVisible = false;

        if (String.IsNullOrWhiteSpace(txtProdName.Text))
        {
            lblNameEmpty.IsVisible = true;
            b = false;
        }
        if (pckCategory.SelectedItem == null)
        {
            lblCategoryEmpty.IsVisible = true;
            b = false;
        }
        if (String.IsNullOrWhiteSpace(txtCost.Text))
        {
            lblCostEmpty.IsVisible = true;
            b = false;
        }
        if (String.IsNullOrWhiteSpace(txtDescription.Text))
        {
            lblDescriptionEmpty.IsVisible = true;
            b = false;
        }

        if (!b) return;

        await Cropper.CropImageAsync(false);
        MemoryStream ms = new();
        ms.Write(Cropper.CroppedImageBytes);
        ms.Position = 0;

        imgStream = ms;

        if (imgStream.Length == 0)
        {
            lblImageEmpty.IsVisible = true;
            b = false;
        }

        if (!await ProdVM.CheckProdName(txtProdName.Text, "") && b)
        {
            await Shell.Current.DisplayAlert("Error!", "Product name already exists...", "OK");
            return;
        }

        if (b)
        {
            var addProd = await Shell.Current.DisplayAlert("", "Create new product?", "Yes", "No");
            if (addProd)
            {
                ProdVM.loadingPopup = new MauiToolkitPopupSample._0PopupLoadingPage();
                this.ShowPopup(ProdVM.loadingPopup);
                
                ProdVM.AddProduct(imgStream, txtProdName.Text, Int32.Parse(txtCost.Text), txtDescription.Text, pckCategory.SelectedIndex);
            }
        }
    }
}