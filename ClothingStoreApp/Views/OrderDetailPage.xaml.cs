using ClothingStoreApp.ViewModels;
using ClothingStoreApp.Models;
using ClothingStoreApp.Converters;

namespace ClothingStoreApp.Views
{
    public partial class OrderDetailPage : ContentPage
    {
        public OrderDetailPage(Order order)
        {
            InitializeComponent();
            Resources.Add("QuantityPriceConverter", new QuantityPriceConverter());
            BindingContext = new OrderDetailViewModel(order);
        }
    }
}