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

        public ProfileViewModel(SqlService sqlService)
        {
            _sqlService = sqlService;
            Orders = new ObservableCollection<Order>();
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

                var orders = _sqlService.GetUserOrders(App.CurrentUserId.Value);
                Orders.Clear(); // Clear trước khi add mới
                foreach (var order in orders)
                {
                    Orders.Add(order);
                }
                System.Diagnostics.Debug.WriteLine($"LoadUserData: Loaded {Orders.Count} orders for UserID={App.CurrentUserId.Value}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadUserData Error: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task ViewUserInfo()
        {
            // Kiểm tra user đăng nhập trước
            if (!App.CurrentUserId.HasValue)
            {
                await Application.Current.MainPage.DisplayAlert("Lỗi", "Bạn chưa đăng nhập.", "OK");
                return;
            }

            // Thử reload dữ liệu nếu CurrentUser null
            if (CurrentUser == null)
            {
                System.Diagnostics.Debug.WriteLine("ViewUserInfo: CurrentUser is null, trying to reload...");
                LoadUserData();

                // Nếu vẫn null sau khi reload
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
        private async Task ViewOrders()
        {
            // Kiểm tra user đăng nhập trước
            if (!App.CurrentUserId.HasValue)
            {
                await Application.Current.MainPage.DisplayAlert("Lỗi", "Bạn chưa đăng nhập.", "OK");
                return;
            }

            // Thử reload dữ liệu nếu Orders rỗng
            if (Orders.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine("ViewOrders: Orders is empty, trying to reload...");
                LoadUserData();
            }

            // Kiểm tra lại sau khi reload
            if (Orders.Count == 0)
            {
                await Application.Current.MainPage.DisplayAlert("Thông báo", "Bạn chưa có đơn hàng nào.", "OK");
                return;
            }

            string orderSummary = "";
            foreach (var order in Orders)
            {
                orderSummary += $"Đơn hàng #{order.OrderID}\n" +
                                $"Ngày đặt: {order.OrderDate:dd/MM/yyyy HH:mm}\n" +
                                $"Tổng tiền: {order.TotalAmount:C0}\n" +
                                $"Địa chỉ: {order.Address}\n\n";
            }
            await Application.Current.MainPage.DisplayAlert("Lịch sử đơn hàng", orderSummary, "OK");
            System.Diagnostics.Debug.WriteLine("ViewOrders: Displayed order history");
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

        // Thêm method để refresh dữ liệu (giống WishlistViewModel)
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