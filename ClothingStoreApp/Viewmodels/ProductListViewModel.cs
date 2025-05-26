using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using ClothingStoreApp.Models;
using ClothingStoreApp.Services;
using ClothingStoreApp.Views;

namespace ClothingStoreApp.ViewModels
{
    public partial class ProductListViewModel : ObservableObject
    {
        private readonly SqlService _sqlService;

        [ObservableProperty]
        private ObservableCollection<Product> _products;

        [ObservableProperty]
        private bool _isProductListEmpty;

        [ObservableProperty]
        private bool _hasProducts;

        [ObservableProperty]
        private string _searchContext;

        public ProductListViewModel(SqlService sqlService)
        {
            _sqlService = sqlService;
            Products = new ObservableCollection<Product>();
        }

        public void LoadProductsBySearch(string searchQuery)
        {
            SearchContext = string.IsNullOrEmpty(searchQuery)
                ? "Kết quả tìm kiếm"
                : $"Kết quả tìm kiếm cho '{searchQuery}'";
            Products.Clear();
            var products = _sqlService.GetProductsBySearch(searchQuery);
            foreach (var product in products)
            {
                Products.Add(product);
            }
            IsProductListEmpty = Products.Count == 0;
            HasProducts = Products.Count > 0;
            System.Diagnostics.Debug.WriteLine($"LoadProductsBySearch: Query='{searchQuery}', Found {Products.Count} products.");
        }

        public void LoadProductsByCategory(int categoryId, string categoryName)
        {
            SearchContext = $"Danh mục: {categoryName}";
            Products.Clear();
            var products = _sqlService.GetProductsByCategory(categoryId);
            foreach (var product in products)
            {
                Products.Add(product);
            }
            IsProductListEmpty = Products.Count == 0;
            HasProducts = Products.Count > 0;
            System.Diagnostics.Debug.WriteLine($"LoadProductsByCategory: CategoryID={categoryId}, Name='{categoryName}', Found {Products.Count} products.");
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
        private async Task NavigateBack()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("NavigateBack: Popping navigation stack");
                await Application.Current.MainPage.Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"NavigateBack Error: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Lỗi", $"Lỗi điều hướng: {ex.Message}", "OK");
            }
        }
    }
}