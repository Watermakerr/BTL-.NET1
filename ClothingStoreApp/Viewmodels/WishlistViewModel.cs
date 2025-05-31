using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using ClothingStoreApp.Models;
using ClothingStoreApp.Services;
using ClothingStoreApp.Views;

namespace ClothingStoreApp.ViewModels
{
    public partial class WishlistViewModel : ObservableObject
    {
        private readonly SqlService _sqlService;

        [ObservableProperty]
        private ObservableCollection<Product> _wishlistProducts;

        [ObservableProperty]
        private bool _isWishlistEmpty;

        [ObservableProperty]
        private bool _hasWishlistProducts;

        [ObservableProperty]
        private string _statusMessage;

        public WishlistViewModel(SqlService sqlService)
        {
            _sqlService = sqlService;
            WishlistProducts = new ObservableCollection<Product>();
            LoadWishlistProducts();
        }

        private void LoadWishlistProducts()
        {
            if (!App.CurrentUserId.HasValue)
            {
                IsWishlistEmpty = true;
                HasWishlistProducts = false;
                return;
            }

            var products = _sqlService.GetWishlistProducts(App.CurrentUserId.Value);

            WishlistProducts.Clear();
            foreach (var product in products)
            {
                WishlistProducts.Add(product);
            }

            IsWishlistEmpty = WishlistProducts.Count == 0;
            HasWishlistProducts = WishlistProducts.Count > 0;
            StatusMessage = WishlistProducts.Count == 0 
                ? "Your wishlist is empty" 
                : $"{WishlistProducts.Count} items in wishlist";
        }

        [RelayCommand]
        private async Task NavigateToProductDetail(int productId)
        {
            await Application.Current.MainPage.Navigation.PushAsync(new ProductDetailPage(productId));
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
        private async Task NavigateToProfile(object parameter)
        {
            try
            {
                await Application.Current.MainPage.Navigation.PushAsync(new ProfilePage());
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Lỗi", $"Lỗi điều hướng: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private void RefreshWishlist()
        {
            LoadWishlistProducts();
        }
    }
}