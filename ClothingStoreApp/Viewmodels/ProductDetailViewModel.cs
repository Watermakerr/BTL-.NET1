using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using ClothingStoreApp.Models;
using ClothingStoreApp.Services;

namespace ClothingStoreApp.ViewModels
{
    public partial class ProductDetailViewModel : ObservableObject
    {
        private readonly SqlService _sqlService;

        [ObservableProperty]
        private Product _product;

        [ObservableProperty]
        private ObservableCollection<Review> _reviews;

        [ObservableProperty]
        private bool _isInWishlist;

        [ObservableProperty]
        private string _heartIcon;

        [ObservableProperty]
        private int _quantity = 1;

        public ProductDetailViewModel(int productId, SqlService sqlService)
        {
            _sqlService = sqlService;
            Reviews = new ObservableCollection<Review>();
            LoadProduct(productId);
            LoadReviews(productId);
            CheckWishlistStatus(productId);
        }

        private void LoadProduct(int productId)
        {
            Product = _sqlService.GetProductById(productId);
        }

        private void LoadReviews(int productId)
        {
            var reviews = _sqlService.GetReviewsByProductId(productId);
            foreach (var review in reviews)
            {
                Reviews.Add(review);
            }
        }

        private void CheckWishlistStatus(int productId)
        {
            if (App.CurrentUserId.HasValue)
            {
                IsInWishlist = _sqlService.IsProductInWishlist(App.CurrentUserId.Value, productId);
                HeartIcon = IsInWishlist ? "♥" : "♡";
            }
            else
            {
                IsInWishlist = false;
                HeartIcon = "♡";
            }
        }

        [RelayCommand]
        private async Task ToggleWishlist()
        {
            if (!App.CurrentUserId.HasValue)
            {
                await Application.Current.MainPage.DisplayAlert("Lỗi", "Vui lòng đăng nhập để thêm vào danh sách yêu thích.", "OK");
                return;
            }

            bool success = IsInWishlist 
                ? _sqlService.RemoveFromWishlist(App.CurrentUserId.Value, Product.ProductID)
                : _sqlService.AddToWishlist(App.CurrentUserId.Value, Product.ProductID);

            if (success)
            {
                IsInWishlist = !IsInWishlist;
                HeartIcon = IsInWishlist ? "♥" : "♡";
            }
            else
            {
                string message = IsInWishlist 
                    ? "Không thể xóa khỏi danh sách yêu thích." 
                    : "Không thể thêm vào danh sách yêu thích.";
                await Application.Current.MainPage.DisplayAlert("Lỗi", message, "OK");
            }
        }

        [RelayCommand]
        private async Task AddToCart(object parameter)
        {
            if (!App.CurrentUserId.HasValue)
            {
                await Application.Current.MainPage.DisplayAlert("Lỗi", "Vui lòng đăng nhập để thêm vào giỏ hàng.", "OK");
                return;
            }

            if (parameter is not ContentPage)
            {
                await Application.Current.MainPage.DisplayAlert("Lỗi", "Lỗi điều hướng.", "OK");
                return;
            }

            if (Quantity < 1 || Quantity > 100)
            {
                await Application.Current.MainPage.DisplayAlert("Lỗi", "Số lượng phải từ 1 đến 100.", "OK");
                return;
            }

            try
            {
                bool isInCart = _sqlService.IsProductInCart(App.CurrentUserId.Value, Product.ProductID);
                bool success = isInCart
                    ? _sqlService.UpdateCartItem(App.CurrentUserId.Value, Product.ProductID, Quantity)
                    : _sqlService.AddToCart(App.CurrentUserId.Value, Product.ProductID, Quantity);

                if (success)
                {
                    await Application.Current.MainPage.DisplayAlert("Thành công", "Sản phẩm đã được thêm vào giỏ hàng.", "OK");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Lỗi", "Không thể thêm sản phẩm vào giỏ hàng.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Lỗi", $"Lỗi: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private void IncreaseQuantity()
        {
            if (Quantity < 100)
            {
                Quantity++;
            }
        }

        [RelayCommand]
        private void DecreaseQuantity()
        {
            if (Quantity > 1)
            {
                Quantity--;
            }
        }
    }
}