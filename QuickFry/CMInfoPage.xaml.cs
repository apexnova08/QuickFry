using CommunityToolkit.Maui.Views;
using QuickFry.ViewModels;

namespace QuickFry;

public partial class CMInfoPage : ContentPage
{
    CustomerViewModel CustomerVM = new CustomerViewModel();

    public CMInfoPage()
	{
		InitializeComponent();

        imgProd.Source = App.SelectedProductDisplay.Image;
        lblName.Text = App.SelectedProductDisplay.Name;
        lblCategory.Text = App.SelectedProductDisplay.CategoryName;
        lblCost.Text = App.SelectedProductDisplay.Cost.ToString() + ".00";
        lblDescription.Text = App.SelectedProductDisplay.Description;

        if (App.LoggedInAccout == null)
        {
            LoggedOutPanel.IsVisible = true;
            LoggedInPanel.IsVisible = false;
        }
        else
        {
            LoggedOutPanel.IsVisible = false;
            LoggedInPanel.IsVisible = true;
        }
    }

    private void AddValueChanged(object sender, TextChangedEventArgs e)
    {
        if (Int32.TryParse(txtAddValue.Text, out int num) || String.IsNullOrEmpty(txtAddValue.Text))
        {
            btnValDown.IsEnabled = true;
            btnValUp.IsEnabled = true;

            if (String.IsNullOrEmpty(txtAddValue.Text))
                txtAddValue.Text = "0";
            else if (num < 0)
                txtAddValue.Text = Math.Abs(num).ToString();
            else
                txtAddValue.Text = num.ToString();

            if (num > 0)
                btnAddItem.IsEnabled = true;
            else
                btnAddItem.IsEnabled = false;
        }
        else
        {
            btnValDown.IsEnabled = false;
            btnValUp.IsEnabled = false;

            btnAddItem.IsEnabled = false;
        }
    }
    private void AddValueUp(object sender, EventArgs e)
    {
        if (String.IsNullOrWhiteSpace(txtAddValue.Text))
            txtAddValue.Text = "0";

        txtAddValue.Text = (Int32.Parse(txtAddValue.Text) + 1).ToString();

        //CloseKeyboards();
    }
    private void AddValueDown(object sender, EventArgs e)
    {
        if (String.IsNullOrWhiteSpace(txtAddValue.Text))
            txtAddValue.Text = "0";

        if (!(Int32.Parse(txtAddValue.Text) <= 0))
            txtAddValue.Text = (Int32.Parse(txtAddValue.Text) - 1).ToString();

        //CloseKeyboards();
    }
    private async void AddToCart(object sender, EventArgs e)
    {
        var addToCart = await Shell.Current.DisplayAlert("", "Add to cart?", "Yes", "No");
        if (addToCart)
        {
            CustomerVM.loadingPopup = new MauiToolkitPopupSample._0PopupLoadingPage();
            this.ShowPopup(CustomerVM.loadingPopup);

            CustomerVM.AddToCart(App.SelectedProduct.ID, Int32.Parse(txtAddValue.Text), App.SelectedProductDisplay.Cost * Int32.Parse(txtAddValue.Text));
        }
    }
}