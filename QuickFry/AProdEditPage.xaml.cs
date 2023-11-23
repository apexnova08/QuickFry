using CommunityToolkit.Maui.Views;
using QuickFry.ViewModels;

namespace QuickFry;

public partial class AProdEditPage : ContentPage
{
    ProductViewModel ProdVM = new ProductViewModel();

    bool imgChangeClicked = false;
    Stream imgStream;

    public AProdEditPage()
	{
		InitializeComponent();

        BindingContext = ProdVM;

        txtProdName.Text = App.SelectedProduct.Name;
        ProdImage.Source = App.SelectedProduct.Image;
        txtCost.Text = App.SelectedProduct.Cost.ToString();
        txtDescription.Text = App.SelectedProduct.Description;

        ProdVM.GetCategories();
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
    }

    private void EditImage(object sender, EventArgs e)
    {
        ImgCurrent.IsVisible = false;
        ImgNew.IsVisible = true;

        imgChangeClicked = true;
    }
    private void RevertImage(object sender, EventArgs e)
    {
        ImgCurrent.IsVisible = true;
        ImgNew.IsVisible = false;

        imgChangeClicked = false;
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

        if (txtProdName.Text == App.SelectedProduct.Name && !imgChangeClicked && ProdVM.SelectedCategory.ID == App.SelectedProduct.Category && txtDescription.Text == App.SelectedProduct.Description && txtCost.Text == App.SelectedProduct.Cost.ToString())
        {
            await Shell.Current.DisplayAlert("Error!", "Nothing to update...", "OK");
            return;
        }

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
        if (imgChangeClicked)
        {
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
        }

        if (!await ProdVM.CheckProdName(txtProdName.Text, App.SelectedProduct.ID))
        {
            await Shell.Current.DisplayAlert("Error!", "Product name already exists...", "OK");
            return;
        }

        if (b)
        {
            var addProd = await Shell.Current.DisplayAlert("", "Update product?", "Yes", "No");
            if (addProd)
            {
                ProdVM.loadingPopup = new MauiToolkitPopupSample._0PopupLoadingPage();
                this.ShowPopup(ProdVM.loadingPopup);

                ProdVM.EditProduct(imgStream, imgChangeClicked, txtProdName.Text, Int32.Parse(txtCost.Text), txtDescription.Text, pckCategory.SelectedIndex);
            }
            
        }
    }
}