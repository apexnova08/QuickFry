using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using MauiToolkitPopupSample;
using QuickFry.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace QuickFry.ViewModels;

public partial class AccountViewModel : BaseViewModel
{
    public _0PopupLoadingPage loadingPopup;

    public ObservableCollection<UserAddress> Addresses { get; set; } = new ObservableCollection<UserAddress>();
    public ObservableCollection<Account> AdminAccounts { get; set; } = new ObservableCollection<Account>();
    public ObservableCollection<Account> RiderAccounts { get; set; } = new ObservableCollection<Account>();
    public ObservableCollection<Account> DisabledAccounts { get; set; } = new ObservableCollection<Account>();

    [ObservableProperty]
    private string accountName;
    [ObservableProperty]
    private string accountUsername;
    [ObservableProperty]
    private string accountContact;

    [ObservableProperty]
    private string status;

    public AccountViewModel()
    {
        Status = "Status";
    }

    public async void CheckLogin()
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

            Status = "Checking local login data...";
            string loginText = File.ReadAllText(App.LoggedInAccountPath);

            if (loginText.Contains("N/A"))
            {
                Status = "Redirecting...";
                App.LoggedInAccout = null;
                await Shell.Current.GoToAsync("//CMenuPage");
            }
            else
            {
                string[] loginTextArr = loginText.Split("|");
                DateTime loginExpirationDate = DateTime.ParseExact(loginTextArr[1], "dd/MM/yyyy", null);

                if (DateTime.Now > loginExpirationDate)
                {
                    Status = "Redirecting...";
                    File.WriteAllText(App.LoggedInAccountPath, "N/A");
                    App.LoggedInAccout = null;
                    await Shell.Current.GoToAsync("//CMenuPage");
                }
                else
                {
                    Status = "Logging you in...";
                    var response = await App.client.GetAsync("Accounts/");
                    Dictionary<string, Account> dbAccounts = response.ResultAs<Dictionary<string, Account>>();

                    bool lb = false;
                    foreach (var get in dbAccounts)
                    {
                        if (get.Value.ID == loginTextArr[0])
                        {
                            if (!get.Value.IsActive)
                                await Shell.Current.DisplayAlert("Error!", "Logged in account is disabled.", "OK");
                            else
                            {
                                lb = true;
                                App.LoggedInAccout = get.Value;

                                var rAddresses = await App.client.GetAsync("Addresses/");
                                Dictionary<string, UserAddress> dbAddresses = rAddresses.ResultAs<Dictionary<string, UserAddress>>();
                                foreach (UserAddress address in dbAddresses.Values)
                                {
                                    if (App.LoggedInAccout.ID == address.User && address.IsDefault)
                                    {
                                        App.DefaultAddress = address;
                                        break;
                                    }
                                }

                                if (get.Value.Type == "ADMIN")
                                    await Shell.Current.GoToAsync("//ADashboardPage");
                                else if (get.Value.Type == "RIDER")
                                    await Shell.Current.GoToAsync("//RiderPage");
                                else
                                    await Shell.Current.GoToAsync("//CMenuPage");
                            }
                            break;
                        }
                    }
                    if (!lb)
                    {
                        await Shell.Current.DisplayAlert("Error!", "Account not found.", "OK");

                        App.ClearLogInInfo();
                        await Shell.Current.GoToAsync("//CMenuPage");
                    }
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
        }
    }
    public async void LogIn(string user, string pass)
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

            App.PopupVM.Status = "Logging in...";

            var response = await App.client.GetAsync("Accounts/");
            Dictionary<string, Account> dbAccounts = response.ResultAs<Dictionary<string, Account>>();

            bool lb = false;
            foreach (var get in dbAccounts)
            {
                if (get.Value.Username == user && get.Value.Password == pass)
                {
                    if (!get.Value.IsActive)
                        await Shell.Current.DisplayAlert("Error!", "Logged in account is disabled.", "OK");
                    else
                    {
                        lb = true;
                        App.FillLogInInfo(get.Value);

                        var rAddresses = await App.client.GetAsync("Addresses/");
                        Dictionary<string, UserAddress> dbAddresses = rAddresses.ResultAs<Dictionary<string, UserAddress>>();
                        foreach (UserAddress address in dbAddresses.Values)
                        {
                            if (App.LoggedInAccout.ID == address.User && address.IsDefault)
                            {
                                App.DefaultAddress = address;
                                break;
                            }
                        }

                        if (get.Value.Type == "ADMIN")
                            await Shell.Current.GoToAsync("//ADashboardPage");
                        else if (get.Value.Type == "CUSTOMER")
                            await Shell.Current.GoToAsync("//CMenuPage");
                        else if (get.Value.Type == "RIDER")
                            await Shell.Current.GoToAsync("//RiderPage");
                    }
                    break;
                }
            }
            if (!lb)
                await Shell.Current.DisplayAlert("Error!", "Incorrect email/password.", "OK");
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
    public async void UpdateAccountData()
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

            Status = "Updating account data...";
            var getAccount = App.client.Get("Accounts/" + App.LoggedInAccout.ID);
            App.LoggedInAccout = getAccount.ResultAs<Account>();

            var rAddresses = await App.client.GetAsync("Addresses/");
            Dictionary<string, UserAddress> dbAddresses = rAddresses.ResultAs<Dictionary<string, UserAddress>>();
            foreach (UserAddress address in dbAddresses.Values)
            {
                if (App.LoggedInAccout.ID == address.User && address.IsDefault)
                {
                    App.DefaultAddress = address;
                    break;
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
        }
    }

    [RelayCommand]
    public async Task GetAddressesAsync()
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

            Status = "Clearing collection...";
            if (Addresses.Count != 0)
                Addresses.Clear();

            Status = "Fetching addresses...";
            var rAddresses = await App.client.GetAsync("Addresses/");
            Dictionary<string, UserAddress> dbAddresses = rAddresses.ResultAs<Dictionary<string, UserAddress>>();

            foreach (UserAddress address in dbAddresses.Values)
            {
                if (address.User == App.LoggedInAccout.ID)
                    Addresses.Add(address);
            }

            SortAddresses();
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
    void SortAddresses()
    {
        Status = "Sorting addresses...";
        UserAddress temp = new UserAddress();

        for (int i = 0; i < Addresses.Count; i++)
        {
            if (Addresses[i].IsDefault)
            {
                temp = Addresses[i];

                for (int j = i; j > 0; j--)
                    Addresses[j] = Addresses[j - 1];

                Addresses[0] = temp;
            }
        }
    }
    public async void AddAddress(string name, string address)
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

            App.PopupVM.Status = "Saving address...";
            // Generate ID
            string id = App.GetNowString();

            // Add Address
            UserAddress add = new UserAddress()
            {
                ID = "LOC-" + id,
                User = App.LoggedInAccout.ID,
                Name = name,
                Address = address,
                IsDefault = false
            };
            var setCategory = App.client.Set("Addresses/" + add.ID, add);

            // Exit
            await Shell.Current.GoToAsync($"../");
            await Shell.Current.DisplayAlert("", "New address successfully added.", "OK");
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
    async Task GotoAddressEditAsync(UserAddress address)
    {
        App.SelectedAddress = address;
        await Shell.Current.GoToAsync(nameof(CPAddressEditPage));
    }
    public async void UpdateAddress(string name, string address, bool isDefault)
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

            App.PopupVM.Status = "Updating address...";

            // Update Address
            UserAddress add = new UserAddress()
            {
                ID = App.SelectedAddress.ID,
                User = App.LoggedInAccout.ID,
                Name = name,
                Address = address,
                IsDefault = isDefault
            };

            // If default changed
            if (add.IsDefault && !App.SelectedAddress.IsDefault)
            {
                var rAddresses = await App.client.GetAsync("Addresses/");
                Dictionary<string, UserAddress> dbAddresses = rAddresses.ResultAs<Dictionary<string, UserAddress>>();

                foreach (UserAddress addressval in dbAddresses.Values)
                {
                    if (addressval.IsDefault)
                    {
                        addressval.IsDefault = false;
                        var setAddress = App.client.Update("Addresses/" + addressval.ID, addressval);
                        break;
                    }
                }

                App.DefaultAddress = add;
            }
            var editAddress = App.client.Update("Addresses/" + add.ID, add);

            // Exit
            await Shell.Current.GoToAsync($"../");
            await Shell.Current.DisplayAlert("", "Address successfully updated.", "OK");
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

    public async void RegisterAccount(string user, string pass, string name, string address, string phone, string email)
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

            App.PopupVM.Status = "Checking account availability...";
            var response = await App.client.GetAsync("Accounts/");
            Dictionary<string, Account> dbAccounts = response.ResultAs<Dictionary<string, Account>>();
            foreach (Account account in dbAccounts.Values)
            {
                if (user == account.Username)
                {
                    await Shell.Current.DisplayAlert("Error!", "Username already taken.", "OK");
                    return;
                }
            }

            App.PopupVM.Status = "Creating account...";
            // Generate ID
            string id = App.GetNowString();

            // Add Account
            Account acc = new Account()
            {
                ID = "USER-" + id,
                Username = user,
                Password = pass,
                Name = name,
                Phone = phone,
                Email = email,
                Type = "CUSTOMER",
                IsActive = true
            };
            var setAccount = App.client.Set("Accounts/" + acc.ID, acc);

            // Add Address
            UserAddress add = new UserAddress()
            {
                ID = "LOC-" + id,
                User = acc.ID,
                Name = "Home",
                Address = address,
                IsDefault = true
            };
            var setAddress = App.client.Set("Addresses/" + add.ID, add);

            // Exit
            await Shell.Current.GoToAsync($"../");
            await Shell.Current.DisplayAlert("", "Account successfully created.", "OK");
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
    public async void UpdateAccount(string pass, string name, string phone, string email, string vpass)
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

            var getAccount = App.client.Get("Accounts/" + App.LoggedInAccout.ID);
            if (vpass != getAccount.ResultAs<Account>().Password)
            {
                await Shell.Current.DisplayAlert("Error!", "Incorrect password.", "OK");
                return;
            }

            if (String.IsNullOrEmpty(pass))
                pass = App.LoggedInAccout.Password;

            App.PopupVM.Status = "Updating account details...";

            // Update Account
            Account acc = new Account()
            {
                ID = App.LoggedInAccout.ID,
                Username = App.LoggedInAccout.Username,
                Password = pass,
                Name = name,
                Phone = phone,
                Email = email,
                Type = App.LoggedInAccout.Type,
                IsActive = true
            };
            var editAccount = App.client.Update("Accounts/" + acc.ID, acc);
            App.LoggedInAccout = acc;

            // Exit
            await Shell.Current.GoToAsync($"../");
            await Shell.Current.DisplayAlert("", "Account successfully updated.", "OK");
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
    async Task SelectAddressAsync(UserAddress address)
    {
        App.SelectedAddress = address;
        await Shell.Current.GoToAsync($"../");
    }

    [RelayCommand]
    public async Task GetAccountsAsync()
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

            Status = "Clearing collection...";
            if (AdminAccounts.Count != 0) AdminAccounts.Clear();
            if (RiderAccounts.Count != 0) RiderAccounts.Clear();
            if (DisabledAccounts.Count != 0) DisabledAccounts.Clear();

            Status = "Fetching addresses...";
            var rAccounts = await App.client.GetAsync("Accounts/");
            Dictionary<string, Account> dbAccounts = rAccounts.ResultAs<Dictionary<string, Account>>();

            foreach (Account account in dbAccounts.Values)
            {
                if (!account.IsActive)
                    DisabledAccounts.Add(account);
                if (account.Type == "ADMIN" && account.IsActive)
                    AdminAccounts.Add(account);
                if (account.Type == "RIDER" && account.IsActive)
                    RiderAccounts.Add(account);

                if (account.ID == App.LoggedInAccout.ID)
                {
                    AccountName = account.Name;
                    AccountUsername = account.Username;
                    AccountContact = account.Phone;
                }
            } 

            SortAddresses();
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

    public async void AdminRegisterAccount(string user, string pass, string name, string address, string phone, string email, string type)
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

            App.PopupVM.Status = "Checking account availability...";
            var response = await App.client.GetAsync("Accounts/");
            Dictionary<string, Account> dbAccounts = response.ResultAs<Dictionary<string, Account>>();
            foreach (Account account in dbAccounts.Values)
            {
                if (user == account.Username)
                {
                    await Shell.Current.DisplayAlert("Error!", "Username already taken.", "OK");
                    return;
                }
            }

            App.PopupVM.Status = "Creating account...";
            // Generate ID
            string id = App.GetNowString();

            // Add Account
            Account acc = new Account()
            {
                ID = "USER-" + id,
                Username = user,
                Password = pass,
                Name = name,
                Phone = phone,
                Email = email,
                Type = type,
                IsActive = true
            };
            var setAccount = App.client.Set("Accounts/" + acc.ID, acc);

            // Exit
            await Shell.Current.GoToAsync($"../");
            await Shell.Current.DisplayAlert("", "Account successfully created.", "OK");
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
    public async Task EDAccountAsync(Account account)
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

            App.PopupVM.Status = "Updating account details...";

            Account newaccount = account;

            if (account.IsActive)
                newaccount.IsActive = false;
            else
                newaccount.IsActive = true;

            var editAccount = App.client.Update("Accounts/" + newaccount.ID, newaccount);

            // Exit
            await GetAccountsAsync();
            await Shell.Current.DisplayAlert("", "Account successfully updated.", "OK");
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
