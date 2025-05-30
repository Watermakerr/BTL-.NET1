using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
        private ObservableCollection<OrderItemWrapper> _orderItems;

        [ObservableProperty]
        private string _statusText;

        [ObservableProperty]
        private bool _canCancelOrder;

        [ObservableProperty]
        private decimal _totalPrice;

        public OrderDetailViewModel(Order order)
        {
            _sqlService = App.Services.GetService<SqlService>();
            Order = order;
            OrderItems = new ObservableCollection<OrderItemWrapper>();
            UpdateStatusDisplay();
            LoadOrderItems();
        }

        private void UpdateStatusDisplay()
        {
            StatusText = Order.Status switch
            {
                0 => "Đang chờ xử lý",
                1 => "Đang giao",
                2 => "Đã giao",
                4 => "Đã hủy",
                _ => "Không xác định"
            };
            CanCancelOrder = Order.Status == 0;
        }

        private void LoadOrderItems()
        {
            try
            {
                var items = _sqlService.GetOrderItems(Order.OrderID);
                OrderItems.Clear();
                decimal total = 0;
                foreach (var (cart, product) in items)
                {
                    var wrapped = new OrderItemWrapper(cart, product);
                    OrderItems.Add(wrapped);
                    total += cart.Quantity * product.Price;
                }
                TotalPrice = total;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadOrderItems Error: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task CancelOrder()
        {
            try
            {
                bool isCanceled = _sqlService.UpdateOrderStatus(Order.OrderID, 4);
                if (isCanceled)
                {
                    Order.Status = 4;
                    UpdateStatusDisplay();
                    await Application.Current.MainPage.DisplayAlert("Thông báo", "Đơn hàng đã được hủy.", "OK");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Lỗi", "Không thể hủy đơn hàng. Vui lòng thử lại.", "OK");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"CancelOrder Error: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Lỗi", "Đã xảy ra lỗi khi hủy đơn hàng.", "OK");
            }
        }
    }
}