using ClothingStoreApp.Services;
using ClothingStoreApp.ViewModels;

namespace ClothingStoreApp.Views
{
    public partial class ProductDetailPage : ContentPage
    {
        public ProductDetailPage(int productId)
        {
            InitializeComponent();
            var sqlService = App.Services.GetService<SqlService>();
            BindingContext = new ProductDetailViewModel(productId, sqlService);
        }
    }
}