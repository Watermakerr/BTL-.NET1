using CommunityToolkit.Mvvm.ComponentModel;
using ClothingStoreApp.Models;
using ClothingStoreApp.Services;
using System.Collections.ObjectModel;

namespace ClothingStoreApp.ViewModels
{
    public partial class OrderDetailViewModel : ObservableObject
    {
        private readonly SqlService _sqlService;

        [ObservableProperty]
        private Order _order;

        [ObservableProperty]
        private ObservableCollection<(Cart Cart, Product Product)> _orderItems;

        public OrderDetailViewModel(Order order)
        {
            _sqlService = App.Services.GetService<SqlService>();
            Order = order;
            OrderItems = new ObservableCollection<(Cart Cart, Product Product)>();
            LoadOrderItems();
        }

        private void LoadOrderItems()
        {
            try
            {
                var items = _sqlService.GetOrderItems(Order.OrderID);
                OrderItems.Clear();
                foreach (var item in items)
                {
                    OrderItems.Add(item);
                }
                System.Diagnostics.Debug.WriteLine($"LoadOrderItems: Loaded {OrderItems.Count} items for OrderID={Order.OrderID}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadOrderItems Error: {ex.Message}");
            }
        }
    }
}