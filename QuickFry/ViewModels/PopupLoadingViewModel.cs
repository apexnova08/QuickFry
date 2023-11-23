using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace QuickFry.ViewModels;

public partial class PopupLoadingViewModel : BaseViewModel
{
    [ObservableProperty]
    private string status = "Status";
}
