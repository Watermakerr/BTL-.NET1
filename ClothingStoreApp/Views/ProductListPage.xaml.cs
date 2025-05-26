using ClothingStoreApp.Services;
using ClothingStoreApp.ViewModels;
using Microsoft.Maui.Controls;

namespace ClothingStoreApp.Views
{
    public partial class ProductListPage : ContentPage
    {
        private string _searchQuery;
        private string _categoryId;
        private string _categoryName;

        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                _searchQuery = value;
                System.Diagnostics.Debug.WriteLine($"ProductListPage: SearchQuery set to '{_searchQuery}'");
                LoadProducts();
            }
        }

        public string CategoryId
        {
            get => _categoryId;
            set
            {
                _categoryId = value;
                System.Diagnostics.Debug.WriteLine($"ProductListPage: CategoryId set to '{_categoryId}'");
                LoadProducts();
            }
        }

        public string CategoryName
        {
            get => _categoryName;
            set
            {
                _categoryName = value;
                System.Diagnostics.Debug.WriteLine($"ProductListPage: CategoryName set to '{_categoryName}'");
                LoadProducts();
            }
        }

        public ProductListPage()
        {
            InitializeComponent();
            System.Diagnostics.Debug.WriteLine($"ProductListPage created for UserID: {App.CurrentUserId}");
            try
            {
                var sqlService = App.Services.GetService<SqlService>();
                if (sqlService == null)
                {
                    System.Diagnostics.Debug.WriteLine("ProductListPage: SqlService is null");
                    throw new InvalidOperationException("SqlService not found in DI container.");
                }
                BindingContext = new ProductListViewModel(sqlService);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ProductListPage Initialization Error: {ex.Message}");
                throw;
            }
        }

        private void LoadProducts()
        {
            if (BindingContext is ProductListViewModel viewModel)
            {
                try
                {
                    if (!string.IsNullOrEmpty(_searchQuery))
                    {
                        viewModel.LoadProductsBySearch(_searchQuery);
                    }
                    else if (!string.IsNullOrEmpty(_categoryId) && int.TryParse(_categoryId, out int categoryId))
                    {
                        viewModel.LoadProductsByCategory(categoryId, _categoryName);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"LoadProducts Error: {ex.Message}");
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("LoadProducts: BindingContext is not ProductListViewModel");
            }
        }
    }
}