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
using Firebase.Storage;

namespace QuickFry.ViewModels;

public partial class SystemViewModel : BaseViewModel
{
    public _0PopupLoadingPage loadingPopup;

    [ObservableProperty]
    private int deliveryFee;
    [ObservableProperty]
    private string gCashQR;
    [ObservableProperty]
    private string gCashNumber;

    [ObservableProperty]
    private string status;

    public SystemViewModel()
    {
        Status = "Status";
    }

    [RelayCommand]
    public async Task GetDeliveryFeeAsync()
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
            DeliveryFee = 0;

            Status = "Fetching delivery fee...";
            var rDeliveryFee = await App.client.GetAsync("DeliveryFee");
            DeliveryFee = rDeliveryFee.ResultAs<int>();
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
    [RelayCommand]
    public async Task GetGCashQRAsync()
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

            Status = "Fetching GCash QR...";
            var rGcashQR = await App.client.GetAsync("GCashQR");
            GCashQR = rGcashQR.ResultAs<string>();

            Status = "Fetching GCash Number...";
            var rGcashNumber = await App.client.GetAsync("GCashNumber");
            GCashNumber = rGcashNumber.ResultAs<string>();
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

    [RelayCommand]
    async Task GotoCtgEditAsync(Category ctg)
    {
        App.SelectedCategory = ctg;
        await Shell.Current.GoToAsync(nameof(ASCtgEditPage));
    }

    public async void EditDeliveryFee(int newAmount)
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

            App.PopupVM.Status = "Updating delivery fee...";
            var deliveryFeeUpdate = App.client.Set("DeliveryFee", newAmount);

            await Shell.Current.GoToAsync($"../");
            await Shell.Current.DisplayAlert("", "Delivery fee successfully updated.", "OK");
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

    public async void EditGCashQR(FileResult qr, string number)
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

            App.PopupVM.Status = "Updating GCash QR...";
            if (qr != null)
            {
                
                var task = new FirebaseStorage(App.storageURL,
                    new FirebaseStorageOptions
                    {
                        ThrowOnCancel = true
                    })
                    .Child("Thumbnails")
                    .Child(App.GetNowString())
                    .PutAsync(await qr.OpenReadAsync());

                string imageURL = await task;

                var qrUpdate = App.client.Set("GCashQR", imageURL);
            }

            var numberUpdate = App.client.Set("GCashNumber", number);

            await Shell.Current.GoToAsync($"../");
            await Shell.Current.DisplayAlert("", "GCash info successfully updated.", "OK");
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
