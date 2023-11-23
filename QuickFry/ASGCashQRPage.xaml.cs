using CommunityToolkit.Maui.Views;
using QuickFry.ViewModels;

namespace QuickFry;

public partial class ASGCashQRPage : ContentPage
{
    SystemViewModel SystemVM = new SystemViewModel();

    Stream imgStream;
    FileResult photo;

    public ASGCashQRPage()
	{
		InitializeComponent();

        BindingContext = this;

        GetQR();
    }

    async void GetQR()
    {
        await SystemVM.GetGCashQRAsync();

        imgQR.Source = ImageSource.FromUri(new Uri(SystemVM.GCashQR));
        txtNumber.Text = SystemVM.GCashNumber;
    }

    private async void GetImage(Object sender, EventArgs e)
    {
        photo = await MediaPicker.PickPhotoAsync();

        if (photo == null)
            return;

        imgStream = await photo.OpenReadAsync();
        imgQR.Source = ImageSource.FromStream(() => imgStream);
    }

    private async void UpdateClicked(Object sender, EventArgs e)
    {
        if (imgQR.Source.ToString() == "Uri: " + SystemVM.GCashQR && SystemVM.GCashNumber == txtNumber.Text)
        {
            await Shell.Current.DisplayAlert("Error!", "Nothing to update...", "OK");
            return;
        }

        var editqr = await Shell.Current.DisplayAlert("", "Update GCash information?", "Yes", "No");
        if (editqr)
        {
            SystemVM.loadingPopup = new MauiToolkitPopupSample._0PopupLoadingPage();
            this.ShowPopup(SystemVM.loadingPopup);

            SystemVM.EditGCashQR(photo, txtNumber.Text);
        }
    }
}