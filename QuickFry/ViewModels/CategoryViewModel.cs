using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using QuickFry.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using MauiToolkitPopupSample;

namespace QuickFry.ViewModels;

public partial class CategoryViewModel : BaseViewModel
{
    public _0PopupLoadingPage loadingPopup;

    public ObservableCollection<Category> Ctgs { get; set; } = new ObservableCollection<Category>();

    [ObservableProperty]
    private string status;

    public CategoryViewModel()
    {
        Status = "Status";
    }

    [RelayCommand]
    public async Task GetCategoriesAsync()
    {
        Status = "Initializing...";
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

            Status = "Clearing collection...";
            if (Ctgs.Count != 0)
                Ctgs.Clear();

            Status = "Fetching categories...";
            var prodR = await App.client.GetAsync("Categories/");
            Dictionary<string, Category> dbCtgs = prodR.ResultAs<Dictionary<string, Category>>();

            foreach (var get in dbCtgs)
                Ctgs.Add(get.Value);

            SortCategories();
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

    void SortCategories()
    {
        Status = "Sorting categories...";
        Category temp = new Category();

        for (int z = 0; z < Ctgs.Count - 1; z++)
        {
            for (int i = z + 1; i < Ctgs.Count; i++)
            {
                if (String.Compare(Ctgs[z].Name, Ctgs[i].Name) > 0)
                {
                    temp = Ctgs[z];
                    Ctgs[z] = Ctgs[i];
                    Ctgs[i] = temp;
                }
            }
        }
    }

    public async void AddCategory(string name)
    {
        if (IsBusy)
        {
            return;
        }
        try
        {
            IsBusy = true;

            App.PopupVM.Status = "Checking internet connection...";
            if (!App.CheckInternetConnection())
            {
                await Shell.Current.DisplayAlert("Error", "No internet connection.", "OK");
                return;
            }

            App.PopupVM.Status = "Creating new category...";
            string id = App.GetNowString();

            Category ctg = new Category()
            {
                ID = "CTG-" + id,
                Name = name
            };
            var setCategory = App.client.Set("Categories/" + ctg.ID, ctg);

            await Shell.Current.GoToAsync($"../");
            await Shell.Current.DisplayAlert("", "New menu category successfully added.", "OK");
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

    [RelayCommand]
    async Task GotoCtgEditAsync(Category ctg)
    {
        App.SelectedCategory = ctg;
        await Shell.Current.GoToAsync(nameof(ASCtgEditPage));
    }

    public async void EditCategory(string name)
    {
        if (IsBusy)
        {
            return;
        }
        try
        {
            IsBusy = true;

            App.PopupVM.Status = "Checking internet connection...";
            if (!App.CheckInternetConnection())
            {
                await Shell.Current.DisplayAlert("Error", "No internet connection.", "OK");
                return;
            }

            App.PopupVM.Status = "Updating category...";
            App.SelectedCategory.Name = name;

            var ctgUpdate = App.client.Update("Categories/" + App.SelectedCategory.ID, App.SelectedCategory);

            await Shell.Current.GoToAsync($"../");
            await Shell.Current.DisplayAlert("", "Product size successfully updated.", "OK");
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
