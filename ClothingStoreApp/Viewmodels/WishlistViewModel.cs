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
            System.Diagnostics.Debug.WriteLine($"Loading wishlist for UserID: {App.CurrentUserId}");

            if (!App.CurrentUserId.HasValue)
            {
                IsWishlistEmpty = true;
                HasWishlistProducts = false;
                System.Diagnostics.Debug.WriteLine("No current user ID available");
                return;
            }

            try
            {
                var products = _sqlService.GetWishlistProducts(App.CurrentUserId.Value);

                WishlistProducts.Clear();
                foreach (var product in products)
                {
                    WishlistProducts.Add(product);
                    System.Diagnostics.Debug.WriteLine($"Added to ObservableCollection: {product.ProductName}");
                }

                IsWishlistEmpty = WishlistProducts.Count == 0;
                HasWishlistProducts = WishlistProducts.Count > 0;
                StatusMessage = WishlistProducts.Count == 0 ? "Your wishlist is empty" : $"{WishlistProducts.Count} items in wishlist";

                System.Diagnostics.Debug.WriteLine($"Wishlist loaded: {WishlistProducts.Count} products, IsEmpty: {IsWishlistEmpty}, HasProducts: {HasWishlistProducts}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading wishlist: {ex.Message}");
                StatusMessage = "Error loading wishlist";
                IsWishlistEmpty = true;
                HasWishlistProducts = false;
            }
        }

        [RelayCommand]
        private async Task NavigateToProductDetail(int productId)
        {
            try
            {
                // Use traditional navigation instead of Shell routing
                await Application.Current.MainPage.Navigation.PushAsync(new ProductDetailPage(productId));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Navigation error: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error", "Cannot navigate to product detail", "OK");
            }
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
        private void RefreshWishlist()
        {
            LoadWishlistProducts();
        }
    }
}