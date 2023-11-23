namespace QuickFry;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Products
        Routing.RegisterRoute(nameof(AProdAddPage), typeof(AProdAddPage));
        Routing.RegisterRoute(nameof(AProdInfoPage), typeof(AProdInfoPage));
        Routing.RegisterRoute(nameof(AProdEditPage), typeof(AProdEditPage));

        // Settings
        Routing.RegisterRoute(nameof(ASCategoriesPage), typeof(ASCategoriesPage));
        Routing.RegisterRoute(nameof(ASCtgAddPage), typeof(ASCtgAddPage));
        Routing.RegisterRoute(nameof(ASCtgEditPage), typeof(ASCtgEditPage));
        Routing.RegisterRoute(nameof(ASDeliveryFeePage), typeof(ASDeliveryFeePage));
        Routing.RegisterRoute(nameof(ASGCashQRPage), typeof(ASGCashQRPage));
        Routing.RegisterRoute(nameof(ASAccountsPage), typeof(ASAccountsPage));
        Routing.RegisterRoute(nameof(ASAccountAddPage), typeof(ASAccountAddPage));
        Routing.RegisterRoute(nameof(ASAccountEditPage), typeof(ASAccountEditPage));

        // Orders
        Routing.RegisterRoute(nameof(AOrderInfoPage), typeof(AOrderInfoPage));
        Routing.RegisterRoute(nameof(AOrdersInfoClosedPage), typeof(AOrdersInfoClosedPage));

        // CUSTOMER
        Routing.RegisterRoute(nameof(CRegisterPage), typeof(CRegisterPage));

        Routing.RegisterRoute(nameof(CMInfoPage), typeof(CMInfoPage));

        Routing.RegisterRoute(nameof(CCartEditPage), typeof(CCartEditPage));
        Routing.RegisterRoute(nameof(CCheckOutPage), typeof(CCheckOutPage));
        Routing.RegisterRoute(nameof(COrderInfoPage), typeof(COrderInfoPage));
        Routing.RegisterRoute(nameof(CCPickAddressPage), typeof(CCPickAddressPage));

        Routing.RegisterRoute(nameof(COInfoPage), typeof(COInfoPage));

        Routing.RegisterRoute(nameof(CPAddressesPage), typeof(CPAddressesPage));
        Routing.RegisterRoute(nameof(CPAddressAddPage), typeof(CPAddressAddPage));
        Routing.RegisterRoute(nameof(CPAddressEditPage), typeof(CPAddressEditPage));

        // RIDER
        Routing.RegisterRoute(nameof(ROrderInfoPage), typeof(ROrderInfoPage));

        // ALL USER LEVELS
        Routing.RegisterRoute(nameof(AccountUpdatePage), typeof(AccountUpdatePage));
    }
}
