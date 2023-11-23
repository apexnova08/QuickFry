using CommunityToolkit.Maui.Views;
using QuickFry.ViewModels;

namespace QuickFry;

public partial class CCartEditPage : ContentPage
{
    CartViewModel CartVM = new CartViewModel();

    public CCartEditPage()
	{
        InitializeComponent();

        imgProd.Source = App.SelectedProductDisplay.Image;
        lblName.Text = App.SelectedProductDisplay.Name;
        lblCategory.Text = App.SelectedProductDisplay.CategoryName;
        lblCost.Text = App.SelectedProductDisplay.Cost.ToString();
        lblDescription.Text = App.SelectedProductDisplay.Description;

        txtAddValue.Text = App.SelectedCartItem.Amount.ToString();
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
                btnUpdate.IsEnabled = true;
            else
                btnUpdate.IsEnabled = false;
        }
        else
        {
            btnValDown.IsEnabled = false;
            btnValUp.IsEnabled = false;

            btnUpdate.IsEnabled = false;
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
    private async void UpdateClicked(object sender, EventArgs e)
    {
        if (txtAddValue.Text == App.SelectedCartItem.Amount.ToString())
        {
            await Shell.Current.DisplayAlert("Error!", "Nothing to update...", "OK");
            return;
        }

        var dOutVar = await Shell.Current.DisplayAlert("", "Update item?", "Yes", "No");
        if (dOutVar)
        {
            CartVM.loadingPopup = new MauiToolkitPopupSample._0PopupLoadingPage();
            this.ShowPopup(CartVM.loadingPopup);

            CartVM.UpdateCartItem(Int32.Parse(txtAddValue.Text), App.SelectedProductDisplay.Cost * Int32.Parse(txtAddValue.Text));
        }
    }
    private async void CancelClicked(object sender, EventArgs e)
    {
        var dOutVar = await Shell.Current.DisplayAlert("", "Cancel updating item?", "Yes", "No");
        if (dOutVar)
        {
            await Shell.Current.GoToAsync($"../");
        }
    }
    private async void RemoveClicked(object sender, EventArgs e)
    {
        var dOutVar = await Shell.Current.DisplayAlert("", "Remove item?", "Yes", "No");
        if (dOutVar)
        {
            CartVM.loadingPopup = new MauiToolkitPopupSample._0PopupLoadingPage();
            this.ShowPopup(CartVM.loadingPopup);

            CartVM.DeleteCartItem();
        }
    }
}