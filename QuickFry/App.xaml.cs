using FireSharp;
using FireSharp.Config;
using QuickFry.Models;
using QuickFry.ViewModels;
using System.Globalization;

namespace QuickFry;

public partial class ProductDisplay : Product
{
    public string CategoryName { get; set; }
}
public partial class App : Application
{
    static string filesPath = FileSystem.Current.AppDataDirectory;

    // Account
    public static Account LoggedInAccout;

    public static string LoggedInAccountPath = Path.Combine(filesPath, "QuickFryAccount");


    // Firebase
    public static string dbURL = "https://quickfry20-default-rtdb.asia-southeast1.firebasedatabase.app/";

    // FireSharp
    FirebaseConfig fcon = new FirebaseConfig()
    {
        AuthSecret = "(Firebase auth code here)",
        BasePath = "https://quickfry20-default-rtdb.asia-southeast1.firebasedatabase.app/"
    };
    public static FirebaseClient client;

    // FirebaseStorage
    public static string storageURL = "quickfry20.appspot.com";

    // Global Objects
    public static Product SelectedProduct;
    public static ProductDisplay SelectedProductDisplay;
    public static Category SelectedCategory;
    public static CartItem SelectedCartItem;

    public static OrderDisplay SelectedOrder;

    public static UserAddress DefaultAddress;
    public static UserAddress SelectedAddress;

    // Customer
    public static CustomerViewModel CustomerVM;

    // Popup
    public static PopupLoadingViewModel PopupVM;

    public App()
    {
        // Check Login
        if (!File.Exists(LoggedInAccountPath))
            File.WriteAllText(LoggedInAccountPath, "N/A");

        // Syncfusion
        Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("(Syncfusion license key here)");

        InitializeComponent();

        client = new FirebaseClient(fcon);

        MainPage = new AppShell();

        SelectedProduct = new Product();
        SelectedCategory = new Category();
        DefaultAddress = new UserAddress();
        SelectedAddress = new UserAddress();

        CustomerVM = new CustomerViewModel();

        PopupVM = new PopupLoadingViewModel();
    }

    // Account
    public static void FillLogInInfo(Account account)
    {
        LoggedInAccout = account;
        File.WriteAllText(LoggedInAccountPath, account.ID + "|" + DateTime.Now.AddDays(30).ToString(new CultureInfo("en-GB")).Split(" ")[0]);
    }
    public static void ClearLogInInfo()
    {
        LoggedInAccout = null;
        File.WriteAllText(LoggedInAccountPath, "N/A");
    }

    // Check Internet
    public static bool CheckInternetConnection()
    {
        NetworkAccess accessType = Connectivity.Current.NetworkAccess;

        if (accessType != NetworkAccess.Internet)
            return false;
        else return true;
    }

    public static string GetNow()
    {
        string dt = DateTime.Now.ToString(new CultureInfo("en-GB"));

        return dt;
    }
    public static string GetNowString()
    {
        string[] dt = DateTime.Now.ToString(new CultureInfo("en-GB")).Split(" ");
        string[] d = dt[0].Split("/");
        string[] t = dt[1].Split(":");
        string id = d[2] + d[1] + d[0] + t[0] + t[1] + t[2];

        return id;
    }
}
