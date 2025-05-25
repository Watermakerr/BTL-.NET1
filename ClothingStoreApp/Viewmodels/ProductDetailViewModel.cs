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

            if (IsInWishlist)
            {
                bool success = _sqlService.RemoveFromWishlist(App.CurrentUserId.Value, Product.ProductID);
                if (success)
                {
                    IsInWishlist = false;
                    HeartIcon = "♡";
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Lỗi", "Không thể xóa khỏi danh sách yêu thích.", "OK");
                }
            }
            else
            {
                bool success = _sqlService.AddToWishlist(App.CurrentUserId.Value, Product.ProductID);
                if (success)
                {
                    IsInWishlist = true;
                    HeartIcon = "♥";
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Lỗi", "Không thể thêm vào danh sách yêu thích.", "OK");
                }
            }
        }
    }
}