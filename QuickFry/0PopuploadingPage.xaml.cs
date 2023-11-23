using QuickFry;
using CommunityToolkit.Maui.Views;

namespace MauiToolkitPopupSample;

public partial class _0PopupLoadingPage : Popup
{
    public _0PopupLoadingPage()
    {
        InitializeComponent();
        BindingContext = App.PopupVM;
    }
}