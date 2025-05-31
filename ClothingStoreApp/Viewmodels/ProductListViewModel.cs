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
        }

        [RelayCommand]
        private async Task NavigateToProductDetail(int productId)
        {
            try
            {
                await Application.Current.MainPage.Navigation.PushAsync(new ProductDetailPage(productId));
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Lỗi", $"Lỗi điều hướng: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task NavigateBack()
        {
            await Application.Current.MainPage.Navigation.PopAsync();
        }
    }
}