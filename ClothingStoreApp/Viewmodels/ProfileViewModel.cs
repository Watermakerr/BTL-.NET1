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
            IsOrderListVisible = false;
            LoadUserData();
        }

        private void LoadUserData()
        {
            if (App.CurrentUserId.HasValue)
            {
                CurrentUser = _sqlService.GetUserInfo(App.CurrentUserId.Value);
            }
        }

        private void LoadOrders()
        {
            if (!App.CurrentUserId.HasValue) return;

            var orders = _sqlService.GetUserOrders(App.CurrentUserId.Value);
            Orders.Clear();
            foreach (var order in orders)
            {
                Orders.Add(order);
            }
        }

        [RelayCommand]
        private void ToggleOrderList(object parameter = null)
        {
            if (!IsOrderListVisible)
            {
                LoadOrders();
            }
            IsOrderListVisible = !IsOrderListVisible;
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
        }

        [RelayCommand]
        private async Task ViewOrderDetails(Order order)
        {
            if (order == null)
            {
                await Application.Current.MainPage.DisplayAlert("Lỗi", "Không thể xem chi tiết đơn hàng.", "OK");
                return;
            }

            try
            {
                await Application.Current.MainPage.Navigation.PushAsync(new OrderDetailPage(order));
            }
            catch (Exception ex)
            {
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
        }

        [RelayCommand]
        private async Task NavigateToHome()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new HomePage());
        }

        [RelayCommand]
        private async Task NavigateToCart(object parameter)
        {
            if (parameter is not ContentPage) return;

            try
            {
                await Application.Current.MainPage.Navigation.PushAsync(new CartPage());
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Lỗi", $"Lỗi điều hướng: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task NavigateToWishlist(object parameter)
        {
            if (parameter is not ContentPage page)
            {
                await Application.Current.MainPage.DisplayAlert("Lỗi", "Lỗi điều hướng.", "OK");
                return;
            }
            
            try
            {
                await page.Navigation.PushAsync(new WishlistPage());
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Lỗi", $"Lỗi điều hướng: {ex.Message}", "OK");
            }
        }
    }
}