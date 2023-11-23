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
using Microsoft.Maui.Controls.Shapes;
using MauiToolkitPopupSample;

namespace QuickFry.ViewModels;

public partial class ProductViewModel : BaseViewModel
{
    public _0PopupLoadingPage loadingPopup;

    // Products
    Dictionary<string, Product> dbProducts;
    public ObservableCollection<ProductDisplay> Prods { get; set; } = new ObservableCollection<ProductDisplay>();
    public ObservableCollection<ProductDisplay> Archived { get; set; } = new ObservableCollection<ProductDisplay>();

    // Categories
    Dictionary<string, Category> dbCategories;
    public ObservableCollection<Category> Ctgs { get; set; } = new ObservableCollection<Category>();

    [ObservableProperty]
    private Category selectedCategory;

    [ObservableProperty]
    private string status;

    public ProductViewModel()
    {
        Status = "Status";
    }

    [RelayCommand]
    public async Task GetProductsAsync()
    {
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

            if (Prods.Count != 0) Prods.Clear();
            if (Archived.Count != 0) Archived.Clear();

            if (Ctgs.Count != 0) Ctgs.Clear();

            Dictionary<string, Category> ctgDict = new Dictionary<string, Category>(await GetCtgDict(true));
            foreach (var get in ctgDict)
                Ctgs.Add(get.Value);

            SelectedCategory = Ctgs[0];

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

                if (get.Value.Archived)
                    Archived.Add(prod);
                else
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
        }
    }
    public async void GetCategories()
    {
        Status = "Initializing categories...";
        if (IsBusy)
            return;

        try
        {
            App.PopupVM.Status = "Checking internet connection...";
            if (!App.CheckInternetConnection())
            {
                await Shell.Current.DisplayAlert("Error", "No internet connection.", "OK");
                return;
            }

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
                Ctgs.Add(get.Value);

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
        for (int z = 0; z < Archived.Count - 1; z++)
        {
            for (int i = z + 1; i < Archived.Count; i++)
            {
                if (String.Compare(Archived[z].Name, Archived[i].Name) > 0)
                {
                    ProductDisplay temp = Archived[z];
                    Archived[z] = Archived[i];
                    Archived[i] = temp;
                }
            }
        }
    }

    [RelayCommand]
    async Task SelectProductAsync(Product prod)
    {
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

        App.SelectedProductDisplay = new ProductDisplay() { ID = prod.ID, Name = prod.Name, Image = prod.Image, Cost = prod.Cost, Category = prod.Category, CategoryName = ctg, Description = prod.Description, Archived = prod.Archived };

        await Shell.Current.GoToAsync(nameof(AProdInfoPage));
    }
    public async Task<bool> CheckProdName(string name, string id)
    {
        if (IsBusy)
            return false;
        try
        {
            IsBusy = true;

            var prodR = await App.client.GetAsync("Products/");
            Dictionary<string, Product> dbProds = prodR.ResultAs<Dictionary<string, Product>>();

            foreach (var get in dbProds)
            {
                if (get.Value.Name.ToUpper() == name.ToUpper() && get.Value.ID != id)
                    return false;
            }
            return true;
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
            return false;
        }
        finally
        {
            IsBusy = false;
        }
    }
    public async void AddProduct(Stream imgStream, string name, int cost, string desc, int ctgindex)
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

            App.PopupVM.Status = "Creating product...";

            // Generate ID
            string id = App.GetNowString();

            string imageURL;

            var task = new FirebaseStorage(App.storageURL,
                new FirebaseStorageOptions
                {
                    ThrowOnCancel = true
                })
                .Child("Thumbnails")
                .Child(App.GetNowString())
                .PutAsync(imgStream);

            imageURL = await task;

            // Add Product
            Product prod = new Product()
            {
                ID = "ITEM-" + id,
                Name = name,
                Cost = cost,
                Image = imageURL,
                Description = desc,
                Category = Ctgs[ctgindex].ID,
                Archived = false,
            };
            var setItem = App.client.Set("Products/" + prod.ID, prod);

            // Exit
            await Shell.Current.GoToAsync($"../");
            await Shell.Current.DisplayAlert("", "New menu item successfully created.", "OK");
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
    public async void EditProduct(Stream imgStream, bool imgChanged, string name, int cost, string desc, int ctgindex)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            App.PopupVM.Status = "Checking internet connection...";
            if (!App.CheckInternetConnection())
            {
                await Shell.Current.DisplayAlert("Error", "No internet connection.", "OK");
                return;
            }

            App.PopupVM.Status = "Updating product...";

            string imageURL;
            if (imgChanged)
            {
                var task = new FirebaseStorage(App.storageURL,
                new FirebaseStorageOptions
                {
                    ThrowOnCancel = true
                })
                .Child("Thumbnails")
                .Child(App.GetNowString())
                .PutAsync(imgStream);

                imageURL = await task;
            }
            else
                imageURL = App.SelectedProduct.Image;

            // Add Product
            Product prod = new Product()
            {
                ID = App.SelectedProduct.ID,
                Name = name,
                Cost = cost,
                Image = imageURL,
                Description = desc,
                Category = Ctgs[ctgindex].ID,
                Archived = App.SelectedProduct.Archived
            };
            var setProduct = App.client.Update("Products/" + prod.ID, prod);

            // Exit
            await Shell.Current.GoToAsync($"../");
            await Shell.Current.DisplayAlert("", "Product info successfully updated.", "OK");
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

    public async void DisableProduct(bool b)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            App.PopupVM.Status = "Checking internet connection...";
            if (!App.CheckInternetConnection())
            {
                await Shell.Current.DisplayAlert("Error", "No internet connection.", "OK");
                return;
            }

            App.PopupVM.Status = "Updating product...";

            // Edit Product
            Product prod = App.SelectedProduct;
            prod.Archived = b;

            var setProduct = App.client.Update("Products/" + prod.ID, prod);

            // Exit
            await Shell.Current.GoToAsync($"../");
            await Shell.Current.DisplayAlert("", "Product info successfully updated.", "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
