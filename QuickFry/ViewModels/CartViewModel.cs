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

public partial class CartDisplay : CartItem
{
    public string ProductName { get; set; }
    public int ProductCost { get; set; }
    public string ProductImage { get; set; }
    public string ProductCategory { get; set; }
}
public partial class CartViewModel : BaseViewModel
{
    public _0PopupLoadingPage loadingPopup;

    public ObservableCollection<CartDisplay> CartItems { get; set; } = new ObservableCollection<CartDisplay>();
    Dictionary<string, CartItem> dbCartItems;
    Dictionary<string, Product> dbProducts;
    Dictionary<string, Category> dbCategories;

    [ObservableProperty]
    private bool cartEmpty;
    [ObservableProperty]
    private bool cartNotEmpty;

    [ObservableProperty]
    private int totalItems;
    [ObservableProperty]
    private int totalCost;
    [ObservableProperty]
    private int deliveryFee;

    [ObservableProperty]
    private int grandTotal;

    [ObservableProperty]
    private string status;

    public CartViewModel()
    {
        Status = "Status";
        CartEmpty = false;
    }

    [RelayCommand]
    public async Task GetCartItemsAsync()
    {
        Status = "Initializing...";
        CartEmpty = false;
        CartNotEmpty = false;
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
            if (CartItems.Count != 0) CartItems.Clear();
            TotalItems = 0;
            TotalCost = 0;
            GrandTotal = 0;
            DeliveryFee = 0;

            var rDeliveryFee = await App.client.GetAsync("DeliveryFee");
            DeliveryFee = rDeliveryFee.ResultAs<int>();

            Status = "Fetching categories...";
            var rCategories = await App.client.GetAsync("Categories/");
            dbCategories = rCategories.ResultAs<Dictionary<string, Category>>();

            Status = "Fetching product info...";
            var rProducts = await App.client.GetAsync("Products/");
            dbProducts = rProducts.ResultAs<Dictionary<string, Product>>();

            Status = "Fetching items...";
            var rCartItems = await App.client.GetAsync("CartItems/");

            if (rCartItems.Body != "null")
                dbCartItems = rCartItems.ResultAs<Dictionary<string, CartItem>>();
            else
                dbCartItems = new Dictionary<string, CartItem>();

            foreach (CartItem item in dbCartItems.Values)
            {
                if (App.LoggedInAccout == null)
                    break;
                else if (item.User == App.LoggedInAccout.ID)
                {
                    if (dbProducts[item.Item].Archived)
                    {
                        var deleteCartItem = await App.client.DeleteAsync("CartItems/" + item.ID);
                    }
                    else
                    {
                        CartDisplay cartD = new CartDisplay()
                        {
                            ID = item.ID,
                            Item = item.Item,
                            User = item.User,
                            Amount = item.Amount,
                            SubTotal = item.SubTotal,
                            ProductName = dbProducts[item.Item].Name,
                            ProductCost = dbProducts[item.Item].Cost,
                            ProductImage = dbProducts[item.Item].Image,
                            ProductCategory = dbCategories[dbProducts[item.Item].Category].Name
                        };

                        CartItems.Add(cartD);

                        TotalItems = TotalItems + cartD.Amount;
                        TotalCost = TotalCost + cartD.SubTotal;
                    }
                }
            }
            GrandTotal = TotalCost;

            if (CartItems.Count <= 0)
            {
                CartEmpty = true;
                CartNotEmpty = false;
            }
            else
            {
                CartEmpty = false;
                CartNotEmpty = true;
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

    [RelayCommand]
    async Task GotoCartItemEditAsync(CartDisplay item)
    {
        Status = "Checking internet connection...";
        if (!App.CheckInternetConnection())
        {
            await Shell.Current.DisplayAlert("Error", "No internet connection.", "OK");
            return;
        }

        App.SelectedCartItem = dbCartItems[item.ID];

        Product prod = dbProducts[item.Item];
        App.SelectedProduct = prod;

        string ctg;
        if (prod.Category == "CTG-NONE")
            ctg = "None";
        else
            ctg = dbCategories[prod.Category].Name;
        App.SelectedProductDisplay = new ProductDisplay() { ID = prod.ID, Name = prod.Name, Image = prod.Image, Cost = prod.Cost, Description = prod.Description, Category = prod.Category, CategoryName = ctg };

        await Shell.Current.GoToAsync(nameof(CCartEditPage));
    }

    public async void UpdateCartItem(int amount, int subtotal)
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

            App.PopupVM.Status = "Updating item...";

            // Update Account
            CartItem cart = new CartItem()
            {
                ID = App.SelectedCartItem.ID,
                Item = App.SelectedCartItem.Item,
                User = App.SelectedCartItem.User,
                Amount = amount,
                SubTotal = subtotal
            };
            var editCart = App.client.Update("CartItems/" + cart.ID, cart);

            // Exit
            await Shell.Current.GoToAsync($"../");
            await Shell.Current.DisplayAlert("", "Item successfully updated.", "OK");
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
    public async void DeleteCartItem()
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

            App.PopupVM.Status = "Removing item...";
            var editCart = App.client.DeleteAsync("CartItems/" + App.SelectedCartItem.ID);

            // Exit
            await Shell.Current.GoToAsync($"../");
            await Shell.Current.DisplayAlert("", "Item successfully removed.", "OK");
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

    public async void PlaceOrder(string address, string contact, string type, string paymentMethod, string remarks)
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

            App.PopupVM.Status = "Creating order...";
            // Generate ID
            string id = App.GetNowString();

            // Add Order
            Order order = new Order()
            {
                ID = "ORDER-" + id,
                Date = App.GetNow(),
                User = App.LoggedInAccout.ID,
                Address = address,
                Contact = contact,
                Type = type,
                DeliveryFee = DeliveryFee,
                TotalItems = TotalItems,
                SubtotalCost = TotalCost,
                TotalCost = GrandTotal,
                PaymentMethod = paymentMethod,
                ToPay = GrandTotal,
                IsPaid = false,
                Remarks = remarks,
                Status = "Pending",
                IsClosed = false
            };
            var setOrder = App.client.Set("Orders/" + order.ID, order);

            int itemCounter = 0;
            foreach (CartDisplay item in CartItems)
            {
                OrderItem orderItem = new OrderItem()
                {
                    ID = order.ID + "-" + itemCounter.ToString("D4"),
                    OrderID = order.ID,
                    Item = item.Item,
                    Cost = item.ProductCost,
                    Amount = item.Amount,
                    SubTotal = item.SubTotal
                };
                var setOrderItem = await App.client.SetAsync("OrderItems/" + orderItem.ID, orderItem);
                itemCounter++;

                var deleteCart = await App.client.DeleteAsync("CartItems/" + item.ID);
            }

            // Exit
            await Shell.Current.GoToAsync($"../");
            await Shell.Current.DisplayAlert("", "Order submitted.", "OK");
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
