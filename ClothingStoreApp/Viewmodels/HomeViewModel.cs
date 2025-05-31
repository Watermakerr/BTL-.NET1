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

        [ObservableProperty]
        private string _searchQuery;

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
            Categories.Clear();
            foreach (var category in categories)
            {
                Categories.Add(category);
            }
        }

        private void LoadTopSellingProducts()
        {
            var products = _sqlService.GetTopSellingProducts();
            TopSellingProducts.Clear();
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
            if (parameter is not ContentPage page) return;
            await page.Navigation.PushAsync(new WishlistPage());
        }

        [RelayCommand]
        private async Task Search(object parameter)
        {
            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                await Application.Current.MainPage.DisplayAlert("Lỗi", "Vui lòng nhập từ khóa tìm kiếm.", "OK");
                return;
            }
            if (parameter is not ContentPage page) return;
            
            await page.Navigation.PushAsync(new ProductListPage { SearchQuery = SearchQuery });
        }

        [RelayCommand]
        private async Task NavigateToCategory(int categoryId)
        {
            if (Categories == null || !Categories.Any())
            {
                await Application.Current.MainPage.DisplayAlert("Lỗi", "Không có danh mục nào được tải.", "OK");
                return;
            }

            var category = Categories.FirstOrDefault(c => c.CategoryID == categoryId);
            string categoryName = category?.CategoryName ?? "Danh mục";
            
            await Application.Current.MainPage.Navigation.PushAsync(new ProductListPage 
            { 
                CategoryId = categoryId.ToString(), 
                CategoryName = categoryName 
            });
        }

        [RelayCommand]
        private async Task NavigateToCart(object parameter)
        {
            if (parameter is not ContentPage) return;
            await Application.Current.MainPage.Navigation.PushAsync(new CartPage());
        }

        [RelayCommand]
        private async Task NavigateToProfile(object parameter)
        {
            await Application.Current.MainPage.Navigation.PushAsync(new ProfilePage());
        }
    }
}