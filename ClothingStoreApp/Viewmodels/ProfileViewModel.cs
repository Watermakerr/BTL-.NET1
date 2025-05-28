using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ClothingStoreApp.Models;
using ClothingStoreApp.Services;
using ClothingStoreApp.Views;
using System.Collections.ObjectModel;

namespace ClothingStoreApp.ViewModels
{
    public partial class ProfileViewModel : ObservableObject
    {
        private readonly SqlService _sqlService;

        [ObservableProperty]
        private User _currentUser;

        [ObservableProperty]
        private ObservableCollection<Order> _orders;

        [ObservableProperty]
        private bool _isOrderListVisible;

        public ProfileViewModel(SqlService sqlService)
        {
            _sqlService = sqlService;
            Orders = new ObservableCollection<Order>();
            IsOrderListVisible = false; // Hidden by default
            LoadUserData();
        }

        private void LoadUserData()
        {
            if (!App.CurrentUserId.HasValue)
            {
                System.Diagnostics.Debug.WriteLine("LoadUserData: No user logged in");
                return;
            }

            try
            {
                CurrentUser = _sqlService.GetUserInfo(App.CurrentUserId.Value);

                if (CurrentUser == null)
                {
                    System.Diagnostics.Debug.WriteLine($"LoadUserData: UserID={App.CurrentUserId.Value} not found");
                    return;
                }
                System.Diagnostics.Debug.WriteLine($"LoadUserData: Loaded user {CurrentUser.Username}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadUserData Error: {ex.Message}");
            }
        }

        private void LoadOrders()
        {
            if (!App.CurrentUserId.HasValue)
            {
                System.Diagnostics.Debug.WriteLine("LoadOrders: No user logged in");
                return;
            }

            try
            {
                var orders = _sqlService.GetUserOrders(App.CurrentUserId.Value);
                Orders.Clear();
                foreach (var order in orders)
                {
                    Orders.Add(order);
                }
                System.Diagnostics.Debug.WriteLine($"LoadOrders: Loaded {Orders.Count} orders for UserID={App.CurrentUserId.Value}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadOrders Error: {ex.Message}");
            }
        }

        [RelayCommand]
        private void ToggleOrderList(object parameter = null)
        {
            System.Diagnostics.Debug.WriteLine($"ToggleOrderList: Parameter={(parameter != null ? parameter.ToString() : "null")}");
            if (!IsOrderListVisible)
            {
                LoadOrders(); // Load orders when showing the list
            }
            IsOrderListVisible = !IsOrderListVisible;
            System.Diagnostics.Debug.WriteLine($"ToggleOrderList: IsOrderListVisible={IsOrderListVisible}");
        }

        [RelayCommand]
        private async Task ViewUserInfo()
        {
            if (!App.CurrentUserId.HasValue)
            {
                await Application.Current.MainPage.DisplayAlert("Lỗi", "Bạn chưa đăng nhập.", "OK");
                return;
            }

            if (CurrentUser == null)
            {
                System.Diagnostics.Debug.WriteLine("ViewUserInfo: CurrentUser is null, trying to reload...");
                LoadUserData();

                if (CurrentUser == null)
                {
                    await Application.Current.MainPage.DisplayAlert("Lỗi", "Không thể tải thông tin người dùng.", "OK");
                    return;
                }
            }

            string userInfo = $"Tên người dùng: {CurrentUser.Username}\n" +
                             $"Số điện thoại: {CurrentUser.PhoneNumber ?? "Chưa có"}\n" +
                             $"Địa chỉ: {CurrentUser.Address ?? "Chưa có"}";
            await Application.Current.MainPage.DisplayAlert("Thông tin cá nhân", userInfo, "OK");
            System.Diagnostics.Debug.WriteLine("ViewUserInfo: Displayed user info");
        }

        [RelayCommand]
        private async Task ViewOrderDetails(Order order)
        {
            if (order == null)
            {
                System.Diagnostics.Debug.WriteLine("ViewOrderDetails: Order is null");
                await Application.Current.MainPage.DisplayAlert("Lỗi", "Không thể xem chi tiết đơn hàng.", "OK");
                return;
            }

            try
            {
                System.Diagnostics.Debug.WriteLine($"ViewOrderDetails: Navigating to OrderDetailPage for OrderID={order.OrderID}");
                await Application.Current.MainPage.Navigation.PushAsync(new OrderDetailPage(order));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ViewOrderDetails Error: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Lỗi", $"Lỗi điều hướng: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task Logout()
        {
            App.CurrentUserId = null;
            await Application.Current.MainPage.DisplayAlert("Thông báo", "Bạn đã đăng xuất thành công.", "OK");
            await Application.Current.MainPage.Navigation.PopToRootAsync();
            Application.Current.MainPage = new NavigationPage(new LoginPage());
            System.Diagnostics.Debug.WriteLine("Logout: User logged out");
        }

        [RelayCommand]
        private void RefreshProfile()
        {
            LoadUserData();
        }

        [RelayCommand]
        private async Task Changepassword()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new ChangePasswordPage());
            System.Diagnostics.Debug.WriteLine("Changepassword: Navigated to ChangePasswordPage");
        }

        [RelayCommand]
        private async Task NavigateToHome()
        {
            try
            {
                await Application.Current.MainPage.Navigation.PushAsync(new HomePage());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Navigation error: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error", "Cannot navigate to home", "OK");
            }
        }

        [RelayCommand]
        private async Task NavigateToCart(object parameter)
        {
            if (parameter is not ContentPage)
            {
                System.Diagnostics.Debug.WriteLine("NavigateToCart: Parameter is not ContentPage");
                return;
            }

            try
            {
                System.Diagnostics.Debug.WriteLine("NavigateToCart: Navigating to CartPage");
                await Application.Current.MainPage.Navigation.PushAsync(new CartPage());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"NavigateToCart Error: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Lỗi", $"Lỗi điều hướng: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task NavigateToWishlist(object parameter)
        {
            if (parameter is not ContentPage page)
            {
                System.Diagnostics.Debug.WriteLine("NavigateToWishlist: Parameter is not ContentPage");
                await Application.Current.MainPage.DisplayAlert("Lỗi", "Lỗi điều hướng.", "OK");
                return;
            }
            try
            {
                System.Diagnostics.Debug.WriteLine("NavigateToWishlist: Navigating to WishlistPage");
                await page.Navigation.PushAsync(new WishlistPage());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"NavigateToWishlist Error: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Lỗi", $"Lỗi điều hướng: {ex.Message}", "OK");
            }
        }
    }
}