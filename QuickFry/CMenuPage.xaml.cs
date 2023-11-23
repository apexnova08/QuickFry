using QuickFry.ViewModels;

namespace QuickFry;

public partial class CMenuPage : ContentPage
{
    CustomerViewModel CVM = new CustomerViewModel();
    string searchString;

    public CMenuPage()
	{
		InitializeComponent();

        Resources["ImageHeight"] = DeviceDisplay.MainDisplayInfo.Width / 2 - 20;

        BindingContext = CVM;
        
        btnClearSearchText.IsVisible = false;/*
        lblSearchResultPanel.HeightRequest = 0;*/
        evItems.IsVisible = false;

        GetProducts();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        CloseKeyboards();
    }

    private void Refreshed(object sender, EventArgs e)
    {/*
        txtSearch.Text = "";
        cbCategory.SelectedItem = null;

        lblSearchResultPanel.HeightRequest = 0;
        evItems.IsVisible = false;*/
    }
    async void GetProducts()
    {
        await CVM.GetProductsAsync();
    }
    private void CloseKeyboards()
    {/*
        txtSearch.IsEnabled = false;
        txtSearch.IsEnabled = true;*/
    }

    private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
    {/*
        if (String.IsNullOrEmpty(txtSearch.Text))
            btnClearSearchText.IsVisible = false;
        else
            btnClearSearchText.IsVisible = true;*/
    }
    private void ClearSearchText(object sender, EventArgs e)
    {
        //txtSearch.Text = null;
    }
    private void SearchClicked(object sender, EventArgs e)
    {/*
        if (txtSearch.Text == null)
            searchString = "";
        else
            searchString = txtSearch.Text;

        SearchProds();*/

        CVM.FilterProducts();
    }
    private void cbCategorySelectionChanged(object sender, Syncfusion.Maui.Inputs.SelectionChangedEventArgs e)
    {/*
        if (cbCategory.SelectedItem != null)
            SearchProds();*/
    }
    private void ClearSearch(object sender, EventArgs e)
    {/*
        txtSearch.Text = null;
        searchString = "";
        SearchProds();*/
    }
    private void SearchProds()
    {/*
        if (String.IsNullOrEmpty(searchString))
        {
            searchString = "";
            lblSearchResultPanel.HeightRequest = 0;
        }
        else
        {
            lblSearchResult.Text = String.Format("Search results for \"{0}\"...", searchString);
            lblSearchResultPanel.HeightRequest = 40;
        }
        if (cbCategory.SelectedItem == null)
            cbCategory.SelectedIndex = 0;

        //ProdVM.FilterProducts(searchString, cbCategory.SelectedIndex);

        if (ProdVM.Prods.Count() <= 0 && ProdVM.Archived.Count() <= 0)
            evItems.IsVisible = true;
        else
            evItems.IsVisible = false;

        CloseKeyboards();*/

        CVM.FilterProducts();
    }
}