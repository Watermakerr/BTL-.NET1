using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using ClothingStoreApp.Models;
using ClothingStoreApp.Services;
using ClothingStoreApp.Views;

namespace ClothingStoreApp.ViewModels
{
    public partial class HomeViewModel : ObservableObject
    {
        private readonly SqlService _sqlService;

        [ObservableProperty]
        private ObservableCollection<Category> _categories;

        [ObservableProperty]
        private ObservableCollection<Product> _topSellingProducts;

        public HomeViewModel(SqlService sqlService)
        {
            _sqlService = sqlService;
            Categories = new ObservableCollection<Category>();
            TopSellingProducts = new ObservableCollection<Product>();
            LoadCategories();
            LoadTopSellingProducts();
        }

        private void LoadCategories()
        {
            var categories = _sqlService.GetCategories();
            foreach (var category in categories)
            {
                Categories.Add(category);
            }
        }

        private void LoadTopSellingProducts()
        {
            var products = _sqlService.GetTopSellingProducts();
            foreach (var product in products)
            {
                TopSellingProducts.Add(product);
            }
        }

        [RelayCommand]
        private async Task NavigateToProductDetail(int productId)
        {
            await Application.Current.MainPage.Navigation.PushAsync(new ProductDetailPage(productId));
        }

        [RelayCommand]
        private async Task NavigateToWishlist(object parameter)
        {
            if (parameter is not ContentPage page)
            {
                await Application.Current.MainPage.DisplayAlert("Lỗi", "Lỗi điều hướng.", "OK");
                return;
            }
            await page.Navigation.PushAsync(new WishlistPage());
        }

        [RelayCommand]
        private async Task NavigateToRegister(object parameter)
        {
            if (parameter is not ContentPage page)
            {
                await Application.Current.MainPage.DisplayAlert("Lỗi", "Lỗi điều hướng.", "OK");
                return;
            }
            await page.Navigation.PushAsync(new RegisterPage());
        }
    }
}