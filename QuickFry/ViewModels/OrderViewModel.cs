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

public partial class OrderDisplay : Order
{
    public string UserName { get; set; }
    public string UserUsername { get; set; }
    public string PaidStatus { get; set; }
    public Color PaidStatusColor { get; set; }
}
public partial class OrderItemDisplay : OrderItem
{
    public string ProductName { get; set; }
    public string ProductImage { get; set; }
    public string ProductCategory { get; set; }
}
public partial class OrderViewModel : BaseViewModel
{
    public _0PopupLoadingPage loadingPopup;

    public ObservableCollection<OrderDisplay> Orders2 { get; set; } = new ObservableCollection<OrderDisplay>();
    public ObservableCollection<OrderDisplay> Orders { get; set; } = new ObservableCollection<OrderDisplay>();

    Dictionary<string, Order> dbOrders;
    Dictionary<string, OrderDisplay> dbOrderDisplay;

    public ObservableCollection<OrderItemDisplay> OrderItems { get; set; } = new ObservableCollection<OrderItemDisplay>();
    Dictionary<string, OrderItem> dbOrderItems;

    [ObservableProperty]
    private bool orderEmpty;

    [ObservableProperty]
    private bool order1NotEmpty;
    [ObservableProperty]
    private bool order2NotEmpty;

    [ObservableProperty]
    private string orderStatus;
    [ObservableProperty]
    private int toPay;
    [ObservableProperty]
    private int paidCash;
    [ObservableProperty]
    private int paidGCash;
    [ObservableProperty]
    private int change;

    [ObservableProperty]
    private string number;
    [ObservableProperty]
    private string qR;

    [ObservableProperty]
    private int grandTotal;

    [ObservableProperty]
    private int outForDelivery;
    [ObservableProperty]
    private int unpaid;
    [ObservableProperty]
    private int totalOrders;

    [ObservableProperty]
    private string status;

    public OrderViewModel()
    {
        Status = "Status";
        OutForDelivery = 0;
        Unpaid = 0;
        TotalOrders = 0;
    }

    [RelayCommand]
    public async Task GetOrdersDashboardAsync()
    {
        Status = "Initializing...";
        OrderEmpty = false;
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
            if (Orders.Count != 0) Orders.Clear();

            OutForDelivery = 0;
            Unpaid = 0;
            TotalOrders = 0;

            dbOrderDisplay = await GetOrdersDict();

            foreach (OrderDisplay order in dbOrderDisplay.Values)
            {
                if (order.Status == "Out for Delivery")
                    OutForDelivery = OutForDelivery + 1;
                if (!order.IsPaid)
                    Unpaid = Unpaid + 1;
                TotalOrders++;
                if (order.Status == "Pending")
                    Orders.Add(order);
            }

            if (Orders.Count > 0) OrderEmpty = false;
            else OrderEmpty = true;
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
    [RelayCommand]
    public async Task GetOrdersUserAsync()
    {
        Status = "Initializing...";
        OrderEmpty = false;
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
            if (Orders.Count != 0) Orders.Clear();

            dbOrderDisplay = await GetOrdersDict();

            foreach (OrderDisplay order in dbOrderDisplay.Values)
            {
                if (App.LoggedInAccout == null)
                    break;
                else if (order.User == App.LoggedInAccout.ID && (order.Status == "Pending" || order.Status == "Preparing" || order.Status == "Out for Delivery" || order.Status == "Ready for Pickup"))
                    Orders.Add(order);
            }

            if (Orders.Count > 0) OrderEmpty = false;
            else OrderEmpty = true;
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
    [RelayCommand]
    public async Task GetOrdersActiveAsync()
    {
        Status = "Initializing...";
        OrderEmpty = false;
        Order1NotEmpty = false;
        Order2NotEmpty = false;
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
            if (Orders2.Count != 0) Orders2.Clear();
            if (Orders.Count != 0) Orders.Clear();

            dbOrderDisplay = await GetOrdersDict();

            foreach (OrderDisplay order in dbOrderDisplay.Values)
            {
                if (order.Status == "Pending")
                    Orders2.Add(order);
                else if (order.Status == "Preparing")
                    Orders.Add(order);
            }

            if (Orders.Count <= 0 && Orders2.Count <= 0) OrderEmpty = true;
            if (Orders.Count > 0) Order1NotEmpty = true;
            if (Orders2.Count > 0) Order2NotEmpty = true;
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
    [RelayCommand]
    public async Task GetOrdersFinishedAsync()
    {
        Status = "Initializing...";
        OrderEmpty = false;
        Order1NotEmpty = false;
        Order2NotEmpty = false;
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
            if (Orders2.Count != 0) Orders2.Clear();
            if (Orders.Count != 0) Orders.Clear();

            dbOrderDisplay = await GetOrdersDict();

            foreach (OrderDisplay order in dbOrderDisplay.Values)
            {
                if (order.Status == "Ready for Pickup")
                    Orders2.Add(order);
                else if (order.Status == "Out for Delivery")
                    Orders.Add(order);
            }

            if (Orders.Count <= 0 && Orders2.Count <= 0) OrderEmpty = true;
            if (Orders.Count > 0) Order1NotEmpty = true;
            if (Orders2.Count > 0) Order2NotEmpty = true;
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
    [RelayCommand]
    public async Task GetOrdersClosedAsync()
    {
        Status = "Initializing...";
        OrderEmpty = false;
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
            if (Orders.Count != 0) Orders.Clear();

            dbOrderDisplay = await GetOrdersDict();

            foreach (OrderDisplay order in dbOrderDisplay.Values)
            {
                if (App.LoggedInAccout == null)
                    break;
                else if (order.IsClosed)
                    Orders.Add(order);
            }

            if (Orders.Count > 0) OrderEmpty = false;
            else OrderEmpty = true;
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
    [RelayCommand]
    public async Task GetOrdersRiderAsync()
    {
        Status = "Initializing...";
        OrderEmpty = false;
        Order1NotEmpty = false;
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
            if (Orders.Count != 0) Orders.Clear();

            dbOrderDisplay = await GetOrdersDict();

            foreach (OrderDisplay order in dbOrderDisplay.Values)
            {
                if (order.Status == "Out for Delivery")
                    Orders.Add(order);
            }

            if (Orders.Count <= 0) OrderEmpty = true;
            if (Orders.Count > 0) Order1NotEmpty = true;
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

    async Task<Dictionary<string, OrderDisplay>> GetOrdersDict()
    {
        List<OrderDisplay> OrderList = new List<OrderDisplay>();

        Status = "Fetching accounts...";
        var rAccounts = await App.client.GetAsync("Accounts/");
        Dictionary<string, Account> dbAccounts = rAccounts.ResultAs<Dictionary<string, Account>>();

        Status = "Fetching orders...";
        var rOrders = await App.client.GetAsync("Orders/");

        if (rOrders.Body != "null")
            dbOrders = rOrders.ResultAs<Dictionary<string, Order>>();
        else
            dbOrders = new Dictionary<string, Order>();

        foreach (Order order in dbOrders.Values)
        {
            OrderDisplay orderdisplay = new OrderDisplay()
            {
                ID = order.ID,
                Date = order.Date,
                User = order.User,
                Address = order.Address,
                Contact = order.Contact,
                Type = order.Type,
                DeliveryFee = order.DeliveryFee,
                TotalItems = order.TotalItems,
                SubtotalCost = order.SubtotalCost,
                TotalCost = order.TotalCost,
                PaymentMethod = order.PaymentMethod,
                PaidCash = order.PaidCash,
                PaidGCash = order.PaidGCash,
                Change = order.Change,
                GCashTNumber = order.GCashTNumber,
                ToPay = order.ToPay,
                IsPaid = order.IsPaid,
                Remarks = order.Remarks,
                ReasonForClosing = order.ReasonForClosing,
                Status = order.Status,
                UserName = dbAccounts[order.User].Name,
                UserUsername = dbAccounts[order.User].Username,
                IsClosed = order.IsClosed
            };

            if (order.IsPaid)
            {
                orderdisplay.PaidStatus = "Paid";
                orderdisplay.PaidStatusColor = Colors.Green;
            }
            else
            {
                orderdisplay.PaidStatus = "Unpaid";
                orderdisplay.PaidStatusColor = Colors.Red;
            }

            OrderList.Add(orderdisplay);
        }

        Dictionary<string, OrderDisplay> OrderDict = new Dictionary<string, OrderDisplay>();
        for (int i = OrderList.Count - 1; i >= 0; i--)
            OrderDict.Add(OrderList[i].ID, OrderList[i]);

        return OrderDict;
    }

    [RelayCommand]
    async Task GotoOrderInfoAdminAsync(OrderDisplay order)
    {
        if (!App.CheckInternetConnection())
        {
            await Shell.Current.DisplayAlert("Error", "No internet connection.", "OK");
            return;
        }

        App.SelectedOrder = order;
        await Shell.Current.GoToAsync(nameof(AOrderInfoPage));
    }
    [RelayCommand]
    async Task GotoOrderInfoClosedAsync(OrderDisplay order)
    {
        if (!App.CheckInternetConnection())
        {
            await Shell.Current.DisplayAlert("Error", "No internet connection.", "OK");
            return;
        }

        App.SelectedOrder = order;
        await Shell.Current.GoToAsync(nameof(AOrdersInfoClosedPage));
    }
    [RelayCommand]
    async Task GotoOrderInfoCustomerAsync(OrderDisplay order)
    {
        if (!App.CheckInternetConnection())
        {
            await Shell.Current.DisplayAlert("Error", "No internet connection.", "OK");
            return;
        }

        App.SelectedOrder = order;
        await Shell.Current.GoToAsync(nameof(COrderInfoPage));
    }
    [RelayCommand]
    async Task GotoOrderInfoRiderAsync(OrderDisplay order)
    {
        if (!App.CheckInternetConnection())
        {
            await Shell.Current.DisplayAlert("Error", "No internet connection.", "OK");
            return;
        }

        App.SelectedOrder = order;
        await Shell.Current.GoToAsync(nameof(ROrderInfoPage));
    }

    [RelayCommand]
    public async Task GetOrderInfoAsync()
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

            OrderStatus = App.SelectedOrder.Status;
            ToPay = App.SelectedOrder.ToPay;
            PaidCash = App.SelectedOrder.PaidCash;
            PaidGCash = App.SelectedOrder.PaidGCash;
            Change = App.SelectedOrder.Change;

            var rQR = await App.client.GetAsync("GCashQR");
            QR = rQR.ResultAs<string>();

            var rNumber = await App.client.GetAsync("GCashNumber");
            Number = rNumber.ResultAs<string>();

            Status = "Clearing collections...";
            if (OrderItems.Count != 0) OrderItems.Clear();

            Status = "Fetching categories...";
            var rCategories = await App.client.GetAsync("Categories/");
            Dictionary<string, Category> dbCategories = rCategories.ResultAs<Dictionary<string, Category>>();

            Status = "Fetching product info...";
            var rProducts = await App.client.GetAsync("Products/");
            Dictionary<string, Product> dbProducts = rProducts.ResultAs<Dictionary<string, Product>>();

            Status = "Fetching items...";
            var rOrderItems = await App.client.GetAsync("OrderItems/");

            if (rOrderItems.Body != "null")
                dbOrderItems = rOrderItems.ResultAs<Dictionary<string, OrderItem>>();
            else
                dbOrderItems = new Dictionary<string, OrderItem>();

            foreach (OrderItem item in dbOrderItems.Values)
            {
                if (item.OrderID == App.SelectedOrder.ID)
                {
                    OrderItemDisplay orderD = new OrderItemDisplay()
                    {
                        ID = item.ID,
                        OrderID = item.OrderID,
                        Item = item.Item,
                        Cost = item.Cost,
                        Amount = item.Amount,
                        SubTotal = item.SubTotal,
                        ProductName = dbProducts[item.Item].Name,
                        ProductImage = dbProducts[item.Item].Image,
                        ProductCategory = dbCategories[dbProducts[item.Item].Category].Name
                    };

                    OrderItems.Add(orderD);
                }
            }
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

    public async void UpdateOrderGCashTransactionNumber(string transactionnumber)
    {
        App.PopupVM.Status = "Initializing...";
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

            App.PopupVM.Status = "Checking order...";
            var rOrder = await App.client.GetAsync("Orders/" + App.SelectedOrder.ID);
            Order order = rOrder.ResultAs<Order>();

            if (order.IsClosed)
            {
                await Shell.Current.DisplayAlert("Error", "Order is no longer available.", "OK");
                return;
            }

            App.PopupVM.Status = "Updating order...";
            order.GCashTNumber = transactionnumber;
            var editOrder = App.client.Update("Orders/" + order.ID, order);

            // Exit
            await Shell.Current.GoToAsync($"../");
            await Shell.Current.DisplayAlert("", "Payment updated.", "OK");
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

    public async Task UpdateOrderPayment(int cash, int gcash)
    {
        App.PopupVM.Status = "Initializing...";
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

            App.PopupVM.Status = "Checking order...";
            var rOrder = await App.client.GetAsync("Orders/" + App.SelectedOrder.ID);
            Order order = rOrder.ResultAs<Order>();

            if (order.IsClosed)
            {
                await Shell.Current.DisplayAlert("Error", "Order is no longer available.", "OK");
                return;
            }

            PaidCash = cash;
            PaidGCash = gcash;

            ToPay = order.TotalCost - (cash + gcash);
            if (ToPay < 0)
                ToPay = 0;

            Change = (cash + gcash) - order.TotalCost;
            if (Change < 0)
                Change = 0;


            App.PopupVM.Status = "Updating order...";
            order.PaidCash = cash;
            order.PaidGCash = gcash;
            order.ToPay = ToPay;
            order.Change = Change;
            var editOrder = App.client.Update("Orders/" + order.ID, order);
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
    public async Task UpdateOrderPaymentRider(int cash)
    {
        App.PopupVM.Status = "Initializing...";
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

            App.PopupVM.Status = "Checking order...";
            var rOrder = await App.client.GetAsync("Orders/" + App.SelectedOrder.ID);
            Order order = rOrder.ResultAs<Order>();

            if (order.IsClosed)
            {
                await Shell.Current.DisplayAlert("Error", "Order is no longer available.", "OK");
                return;
            }

            PaidCash = PaidCash + cash;
            

            ToPay = order.TotalCost - (PaidCash + PaidGCash);
            if (ToPay < 0)
                ToPay = 0;

            Change = (PaidCash + PaidGCash) - order.TotalCost;
            if (Change < 0)
                Change = 0;

            App.PopupVM.Status = "Updating order...";
            order.PaidCash = PaidCash;
            order.ToPay = ToPay;
            order.Change = Change;
            var editOrder = App.client.Update("Orders/" + order.ID, order);
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

    public async void CancelOrder()
    {
        App.PopupVM.Status = "Initializing...";
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

            App.PopupVM.Status = "Checking order...";
            var rOrder = await App.client.GetAsync("Orders/" + App.SelectedOrder.ID);
            Order order = rOrder.ResultAs<Order>();

            if (order.IsClosed)
            {
                await Shell.Current.DisplayAlert("Error", "Order is no longer available.", "OK");
                return;
            }
            if (order.Status != "Pending")
            {
                await Shell.Current.DisplayAlert("Error", "Order is no longer available for cancellation.", "OK");
                return;
            }

            App.PopupVM.Status = "Cancelling order...";
            order.Status = "Cancelled";
            order.IsClosed = true;
            var editOrder = App.client.Update("Orders/" + order.ID, order);

            // Exit
            await Shell.Current.GoToAsync($"../");
            await Shell.Current.DisplayAlert("", "Order cancelled.", "OK");
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
    public async void RejectOrder()
    {
        App.PopupVM.Status = "Initializing...";
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

            App.PopupVM.Status = "Checking order...";
            var rOrder = await App.client.GetAsync("Orders/" + App.SelectedOrder.ID);
            Order order = rOrder.ResultAs<Order>();

            if (order.IsClosed)
            {
                await Shell.Current.DisplayAlert("Error", "Order is no longer available.", "OK");
                return;
            }
            if (order.Status != "Pending")
            {
                await Shell.Current.DisplayAlert("Error", "Order is no longer available for rejection.", "OK");
                return;
            }

            App.PopupVM.Status = "Rejecting order...";
            order.Status = "Rejected";
            order.IsClosed = true;
            var editOrder = App.client.Update("Orders/" + order.ID, order);

            // Exit
            await Shell.Current.GoToAsync($"../");
            await Shell.Current.DisplayAlert("", "Order rejected.", "OK");
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
    public async void AcceptOrder()
    {
        App.PopupVM.Status = "Initializing...";
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

            App.PopupVM.Status = "Checking order...";
            var rOrder = await App.client.GetAsync("Orders/" + App.SelectedOrder.ID);
            Order order = rOrder.ResultAs<Order>();

            if (order.IsClosed)
            {
                await Shell.Current.DisplayAlert("Error", "Order is no longer available.", "OK");
                return;
            }

            App.PopupVM.Status = "Accepting order...";
            order.Status = "Preparing";
            var editOrder = App.client.Update("Orders/" + order.ID, order);

            // Exit
            await Shell.Current.GoToAsync($"../");
            await Shell.Current.DisplayAlert("", "Order accepted.", "OK");
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

    public async void OrderPrepared()
    {
        App.PopupVM.Status = "Initializing...";
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

            App.PopupVM.Status = "Checking order...";
            var rOrder = await App.client.GetAsync("Orders/" + App.SelectedOrder.ID);
            Order order = rOrder.ResultAs<Order>();

            if (order.IsClosed)
            {
                await Shell.Current.DisplayAlert("Error", "Order is no longer available.", "OK");
                return;
            }

            App.PopupVM.Status = "Updating order...";
            if (order.Type == "Delivery")
                order.Status = "Out for Delivery";
            if (order.Type == "Pickup")
                order.Status = "Ready for Pickup";
            var editOrder = App.client.Update("Orders/" + order.ID, order);

            // Exit
            await Shell.Current.GoToAsync($"../");
            await Shell.Current.DisplayAlert("", "Order updated.", "OK");
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

    public async void MarkAsPaid()
    {
        App.PopupVM.Status = "Initializing...";
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

            App.PopupVM.Status = "Checking order...";
            var rOrder = await App.client.GetAsync("Orders/" + App.SelectedOrder.ID);
            Order order = rOrder.ResultAs<Order>();

            if (order.IsClosed)
            {
                await Shell.Current.DisplayAlert("Error", "Order is no longer available.", "OK");
                return;
            }

            App.PopupVM.Status = "Updating order...";
            order.IsPaid = true;
            var editOrder = App.client.Update("Orders/" + order.ID, order);

            // Exit
            await Shell.Current.GoToAsync($"../");
            await Shell.Current.DisplayAlert("", "Order updated.", "OK");
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

    public async void OrderDone()
    {
        App.PopupVM.Status = "Initializing...";
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

            App.PopupVM.Status = "Checking order...";
            var rOrder = await App.client.GetAsync("Orders/" + App.SelectedOrder.ID);
            Order order = rOrder.ResultAs<Order>();

            if (order.IsClosed)
            {
                await Shell.Current.DisplayAlert("Error", "Order is no longer available.", "OK");
                return;
            }
            if (!order.IsPaid)
            {
                await Shell.Current.DisplayAlert("Error", "Order is still unpaid.\nYou cannot close this order yet.", "OK");
                return;
            }
            App.PopupVM.Status = "Closing order...";
            if (order.Type == "Delivery")
                order.Status = "Delivered";
            if (order.Type == "Pickup")
                order.Status = "Picked Up";

            order.IsClosed = true;

            var editOrder = App.client.Update("Orders/" + order.ID, order);

            // Exit
            await Shell.Current.GoToAsync($"../");
            await Shell.Current.DisplayAlert("", "Order updated.", "OK");
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
