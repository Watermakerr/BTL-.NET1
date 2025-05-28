using ClothingStoreApp.ViewModels;
using ClothingStoreApp.Models;

namespace ClothingStoreApp.Views
{
    public partial class OrderDetailPage : ContentPage
    {
        public OrderDetailPage(Order order)
        {
            InitializeComponent();
            BindingContext = new OrderDetailViewModel(order);
        }
    }
}