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
            try
            {
                var categories = _sqlService.GetCategories();
                Categories.Clear();
                foreach (var category in categories)
                {
                    Categories.Add(category);
                }
                System.Diagnostics.Debug.WriteLine($"LoadCategories: Loaded {Categories.Count} categories.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadCategories Error: {ex.Message}");
            }
        }

        private void LoadTopSellingProducts()
        {
            try
            {
                var products = _sqlService.GetTopSellingProducts();
                TopSellingProducts.Clear();
                foreach (var product in products)
                {
                    TopSellingProducts.Add(product);
                }
                System.Diagnostics.Debug.WriteLine($"LoadTopSellingProducts: Loaded {TopSellingProducts.Count} products.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadTopSellingProducts Error: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task NavigateToProductDetail(int productId)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"NavigateToProductDetail: Navigating to ProductID={productId}");
                await Application.Current.MainPage.Navigation.PushAsync(new ProductDetailPage(productId));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"NavigateToProductDetail Error: ProductID={productId}, Message={ex.Message}");
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

        [RelayCommand]
        private async Task NavigateToRegister(object parameter)
        {
            if (parameter is not ContentPage page)
            {
                System.Diagnostics.Debug.WriteLine("NavigateToRegister: Parameter is not ContentPage");
                await Application.Current.MainPage.DisplayAlert("Lỗi", "Lỗi điều hướng.", "OK");
                return;
            }
            try
            {
                System.Diagnostics.Debug.WriteLine("NavigateToRegister: Navigating to RegisterPage");
                await page.Navigation.PushAsync(new RegisterPage());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"NavigateToRegister Error: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Lỗi", $"Lỗi điều hướng: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task Search(object parameter)
        {
            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                await Application.Current.MainPage.DisplayAlert("Lỗi", "Vui lòng nhập từ khóa tìm kiếm.", "OK");
                return;
            }
            if (parameter is not ContentPage page)
            {
                System.Diagnostics.Debug.WriteLine($"Search: Parameter is not ContentPage for SearchQuery='{SearchQuery}'");
                await Application.Current.MainPage.DisplayAlert("Lỗi", "Lỗi điều hướng.", "OK");
                return;
            }
            try
            {
                System.Diagnostics.Debug.WriteLine($"Search: Navigating to ProductListPage with SearchQuery='{SearchQuery}'");
                await page.Navigation.PushAsync(new ProductListPage { SearchQuery = SearchQuery });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Search Error: SearchQuery='{SearchQuery}', Message={ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Lỗi", $"Lỗi điều hướng: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task NavigateToCategory(int categoryId)
        {
            if (Categories == null || !Categories.Any())
            {
                System.Diagnostics.Debug.WriteLine($"NavigateToCategory: Categories is null or empty for CategoryID={categoryId}");
                await Application.Current.MainPage.DisplayAlert("Lỗi", "Không có danh mục nào được tải.", "OK");
                return;
            }

            var category = Categories.FirstOrDefault(c => c.CategoryID == categoryId);
            string categoryName = category?.CategoryName ?? "Danh mục";
            try
            {
                System.Diagnostics.Debug.WriteLine($"NavigateToCategory: Navigating to ProductListPage with CategoryID={categoryId}, Name='{categoryName}'");
                await Application.Current.MainPage.Navigation.PushAsync(new ProductListPage { CategoryId = categoryId.ToString(), CategoryName = categoryName });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"NavigateToCategory Error: CategoryID={categoryId}, Message={ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Lỗi", $"Lỗi điều hướng: {ex.Message}", "OK");
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
    }
}