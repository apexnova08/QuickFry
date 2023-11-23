using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using QuickFry.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Firebase.Storage;
using System.Collections.ObjectModel;
using QuickFry;
using MauiToolkitPopupSample;

namespace QuickFry.ViewModels;

public partial class CategoryDisplay : Category
{
    public Color Color { get; set; }
}
public partial class CustomerViewModel : BaseViewModel
{
    public _0PopupLoadingPage loadingPopup;

    public ObservableCollection<Test> Items { get; set; } = new ObservableCollection<Test>();

    // Products
    Dictionary<string, Product> dbProducts;
    public ObservableCollection<ProductDisplay> Prods { get; set; } = new ObservableCollection<ProductDisplay>();
    [ObservableProperty]
    private bool productsEmpty;

    // Categories
    Dictionary<string, Category> dbCategories;
    public ObservableCollection<CategoryDisplay> Ctgs { get; set; } = new ObservableCollection<CategoryDisplay>();

    [ObservableProperty]
    private string searchString;
    [ObservableProperty]
    private CategoryDisplay searchCategory;

    [ObservableProperty]
    private Category selectedCategory;

    [ObservableProperty]
    private string status;

    public CustomerViewModel()
    {
        Status = "Status";
        ProductsEmpty = false;
    }

    [RelayCommand]
    public async Task GetProductsAsync()
    {
        Status = "Initializing...";
        IsRefreshing = true;
        if (IsBusy) return;

        try
        {
            IsBusy = true;

            Status = "Checking internet connection...";
            if (!App.CheckInternetConnection())
            {
                await Shell.Current.DisplayAlert("Error", "No internet connection.", "OK");
                return;
            }

            Status = "Clearing collections...";
            if (Prods.Count != 0) Prods.Clear();
            if (Ctgs.Count != 0) Ctgs.Clear();

            bool firstCtg = true;
            Dictionary<string, Category> ctgDict = new Dictionary<string, Category>(await GetCtgDict(true));
            foreach (var get in ctgDict)
            {
                Color c = Colors.Yellow;

                if (App.Current.Resources.TryGetValue("TertiaryL", out var unselectedColor))
                    c = (Color)unselectedColor;

                if (firstCtg)
                {
                    if (App.Current.Resources.TryGetValue("Primary", out var selectedColor))
                        c = (Color)selectedColor;
                }

                CategoryDisplay ctg = new CategoryDisplay()
                {
                    ID = get.Value.ID,
                    Name = get.Value.Name,
                    Color = c
                };
                Ctgs.Add(ctg);

                firstCtg = false;
            }

            SearchCategory = Ctgs[0];

            Status = "Fetching items...";
            var prodR = await App.client.GetAsync("Products/");
            dbProducts = prodR.ResultAs<Dictionary<string, Product>>();

            foreach (var get in dbProducts)
            {
                ProductDisplay prod = new ProductDisplay()
                {
                    ID = get.Value.ID,
                    Name = get.Value.Name,
                    Cost = get.Value.Cost,
                    Image = get.Value.Image,
                    Category = get.Value.Category,
                    CategoryName = ctgDict[get.Value.Category].Name,
                    Description = get.Value.Description,
                    Archived = get.Value.Archived
                };

                if (!get.Value.Archived)
                    Prods.Add(prod);
            }

            SortProducts();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
            IsRefreshing = false;
        }
    }
    public async void GetCategories()
    {
        Status = "Initializing...";
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            Status = "Checking internet connection...";
            if (!App.CheckInternetConnection())
            {
                await Shell.Current.DisplayAlert("Error", "No internet connection.", "OK");
                return;
            }

            Status = "Clearing collection...";
            if (Ctgs.Count != 0)
                Ctgs.Clear();

            Dictionary<string, Category> ctgDict = new Dictionary<string, Category>(await GetCtgDict(false));
            foreach (var get in ctgDict)
            {
                CategoryDisplay ctg = new CategoryDisplay()
                {
                    ID = get.Value.ID,
                    Name = get.Value.Name
                };
                Ctgs.Add(ctg);

                if (App.SelectedProduct != null && get.Value.ID == App.SelectedProduct.Category)
                    SelectedCategory = get.Value;
            }
        }
        catch (Exception ex)
        {
            //Debug.WriteLine($"Unable to get monkeys: {ex.Message}");
            await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
    public async Task<Dictionary<string, Category>> GetCtgDict(bool getAll)
    {
        List<Category> ctgList = new List<Category>();

        Status = "Fetching categories...";
        var response = await App.client.GetAsync("Categories/");
        dbCategories = response.ResultAs<Dictionary<string, Category>>();

        if (getAll)
            ctgList.Add(new Category() { ID = "CTG-ALL", Name = "All" });

        foreach (var get in dbCategories)
            ctgList.Add(get.Value);

        Status = "Sorting categories...";
        for (int z = 2; z < ctgList.Count - 1; z++)
        {
            for (int i = z + 1; i < ctgList.Count; i++)
            {
                if (String.Compare(ctgList[z].Name, ctgList[i].Name) > 0)
                {
                    Category temp = ctgList[z];
                    ctgList[z] = ctgList[i];
                    ctgList[i] = temp;
                }
            }
        }

        Dictionary<string, Category> ctgDict = new Dictionary<string, Category>();
        foreach (var ctg in ctgList)
            ctgDict.Add(ctg.ID, ctg);

        return ctgDict;
    }
    void SortProducts()
    {
        Status = "Sorting items...";

        for (int z = 0; z < Prods.Count - 1; z++)
        {
            for (int i = z + 1; i < Prods.Count; i++)
            {
                if (String.Compare(Prods[z].Name, Prods[i].Name) > 0)
                {
                    ProductDisplay temp = Prods[z];
                    Prods[z] = Prods[i];
                    Prods[i] = temp;
                }
            }
        }

        if (Prods.Count <= 0)
            ProductsEmpty = true;
        else
            ProductsEmpty = false;
    }

    [RelayCommand]
    async Task SelectProductAsync(Product prod)
    {
        Status = "Checking internet connection...";
        if (!App.CheckInternetConnection())
        {
            await Shell.Current.DisplayAlert("Error", "No internet connection.", "OK");
            return;
        }

        App.SelectedProduct = prod;

        string ctg;
        if (prod.Category == "CTG-NONE")
            ctg = "None";
        else
            ctg = dbCategories[prod.Category].Name;
        App.SelectedProductDisplay = new ProductDisplay() { ID = prod.ID, Name = prod.Name, Image = prod.Image, Cost = prod.Cost, Description = prod.Description, Category = prod.Category, CategoryName = ctg };

        await Shell.Current.GoToAsync(nameof(CMInfoPage));
    }

    [RelayCommand]
    async Task SelectCategoryAsync(CategoryDisplay ctg)
    {
        if (IsBusy)
            return;
        try
        {
            SearchCategory = ctg;

            List<CategoryDisplay> tempCtgs = Ctgs.ToList();
            Ctgs.Clear();

            foreach (CategoryDisplay tempCtg in tempCtgs)
            {
                tempCtg.Color = Colors.Yellow;
                if (ctg.ID == tempCtg.ID)
                {
                    if (App.Current.Resources.TryGetValue("Primary", out var colorval))
                        tempCtg.Color = (Color)colorval;
                }
                else
                {
                    if (App.Current.Resources.TryGetValue("TertiaryL", out var colorval))
                        tempCtg.Color = (Color)colorval;
                }

                Ctgs.Add(tempCtg);
            }

            FilterProducts();
        }
        catch (Exception ex)
        {
            //Debug.WriteLine($"Unable to get monkeys: {ex.Message}");
            await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
    public void FilterProducts()
    {
        if (SearchString == null)
            SearchString = "";
        if (SearchCategory == null)
            SearchCategory = Ctgs[0];

        string ctgID = SearchCategory.ID;

        Dictionary<string, Category> ctgDict = new Dictionary<string, Category>();
        foreach (Category ctg in Ctgs)
            ctgDict.Add(ctg.ID, ctg);

        Status = "Clearing collection...";
        if (Prods.Count != 0)
            Prods.Clear();

        Status = "Searching products...";

        foreach (var get in dbProducts)
        {
            if ((ctgID == get.Value.Category || ctgID == "CTG-ALL") && get.Value.Name.ToLower().Contains(SearchString.ToLower()) && !get.Value.Archived)
            {
                ProductDisplay prod = new ProductDisplay()
                {
                    ID = get.Value.ID,
                    Name = get.Value.Name,
                    Cost = get.Value.Cost,
                    Image = get.Value.Image,
                    Category = get.Value.Category,
                    CategoryName = ctgDict[get.Value.Category].Name,
                    Description = get.Value.Description,
                    Archived = get.Value.Archived
                };

                Prods.Add(prod);
            }
        }

        SortProducts();
    }

    public async void AddToCart(string item, int amount, int subtotal)
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;

            App.PopupVM.Status = "Checking internet connection...";
            if (!App.CheckInternetConnection())
            {
                await Shell.Current.DisplayAlert("Error", "No internet connection.", "OK");
                return;
            }

            // Generate ID
            string id = App.GetNowString();

            // Add Product
            CartItem ci = new CartItem()
            {
                ID = "CI-" + id,
                Item = item,
                User = App.LoggedInAccout.ID,
                Amount = amount,
                SubTotal = subtotal
            };
            var setItem = App.client.Set("CartItems/" + ci.ID, ci);

            // Exit
            await Shell.Current.GoToAsync($"../");
            await Shell.Current.DisplayAlert("", "Successfully added to cart.", "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
        }
        finally
        {
            loadingPopup.Close();
            IsBusy = false;
        }
    }
}
