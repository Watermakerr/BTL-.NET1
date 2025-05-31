using System.Collections.ObjectModel;
using System.ComponentModel;
using ClothingStoreApp.Models;
using ClothingStoreApp.Services;
using ClothingStoreApp.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ClothingStoreApp.ViewModels
{
    public partial class CartViewModel : ObservableObject
    {
        private readonly SqlService _sqlService;

        [ObservableProperty]
        private ObservableCollection<CartItemViewModel> _cartItems;

        [ObservableProperty]
        private decimal _totalCartPrice;

        public CartViewModel(SqlService sqlService)
        {
            _sqlService = sqlService;
            CartItems = new ObservableCollection<CartItemViewModel>();
        }

        public void LoadCartItems()
        {
            CartItems.Clear();
            if (!App.CurrentUserId.HasValue) return;

            var cartItems = _sqlService.GetCartItems(App.CurrentUserId.Value);
            if (cartItems == null || !cartItems.Any()) return;

            foreach (var item in cartItems)
            {
                if (item.Product == null)
                {
                    var placeholderProduct = new Product
                    {
                        ProductID = item.Cart.ProductID,
                        ProductName = $"Sản phẩm không tồn tại (ID: {item.Cart.ProductID})",
                        Price = 0,
                        ImageURL = "resources/images/placeholder.png"
                    };
                    var viewModel = new CartItemViewModel(item.Cart, placeholderProduct, _sqlService, this);
                    viewModel.PropertyChanged += CartItem_PropertyChanged;
                    CartItems.Add(viewModel);
                    continue;
                }
                var cartItemViewModel = new CartItemViewModel(item.Cart, item.Product, _sqlService, this);
                cartItemViewModel.PropertyChanged += CartItem_PropertyChanged;
                CartItems.Add(cartItemViewModel);
            }

            UpdateTotalPrice();
        }

        private void CartItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CartItemViewModel.Quantity) || e.PropertyName == nameof(CartItemViewModel.TotalPrice))
            {
                UpdateTotalPrice();
            }
        }

        private void UpdateTotalPrice()
        {
            TotalCartPrice = CartItems.Sum(item => item.TotalPrice);
        }

        [RelayCommand]
        private async Task PlaceOrder()
        {
            if (!App.CurrentUserId.HasValue)
            {
                await Application.Current.MainPage.DisplayAlert("Lỗi", "Vui lòng đăng nhập để đặt hàng.", "OK");
                return;
            }

            if (!CartItems.Any())
            {
                await Application.Current.MainPage.DisplayAlert("Lỗi", "Giỏ hàng trống.", "OK");
                return;
            }

            await Application.Current.MainPage.Navigation.PushModalAsync(new ConfirmationModalPage(this));
        }

        public async Task ConfirmOrderAsync(string address)
        {
            if (!App.CurrentUserId.HasValue || string.IsNullOrWhiteSpace(address))
            {
                await Application.Current.MainPage.DisplayAlert("Lỗi", "Thông tin không hợp lệ.", "OK");
                return;
            }

            int orderId = _sqlService.PlaceOrder(App.CurrentUserId.Value, TotalCartPrice, address);
            if (orderId > 0)
            {
                foreach (var item in CartItems)
                {
                    _sqlService.AddOrderDetail(orderId, item.Product.ProductID, item.Quantity, item.Product.Price);
                }
                _sqlService.ClearCart(App.CurrentUserId.Value);
                CartItems.Clear();
                UpdateTotalPrice();
                await Application.Current.MainPage.DisplayAlert("Thông báo", "Đơn hàng đã được đặt thành công!", "OK");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Lỗi", "Không thể đặt hàng.", "OK");
            }
        }

        [RelayCommand]
        private async Task NavigateToHome()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new HomePage());
        }

        [RelayCommand]
        private async Task NavigateToProfile(object parameter)
        {
            await Application.Current.MainPage.Navigation.PushAsync(new ProfilePage());
        }

        [RelayCommand]
        private async Task NavigateToWishlist()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new WishlistPage());
        }

        public void RemoveCartItem(CartItemViewModel item)
        {
            if (item == null || !App.CurrentUserId.HasValue) return;

            bool success = _sqlService.RemoveFromCart(App.CurrentUserId.Value, item.Product.ProductID);
            if (success)
            {
                CartItems.Remove(item);
                UpdateTotalPrice();
            }
        }
    }

    public partial class CartItemViewModel : ObservableObject
    {
        private readonly SqlService _sqlService;
        private readonly Cart _cartItem;
        private readonly CartViewModel _parentViewModel;

        [ObservableProperty]
        private Product _product;

        [ObservableProperty]
        private int _quantity;

        [ObservableProperty]
        private decimal _totalPrice;

        public CartItemViewModel(Cart cartItem, Product product, SqlService sqlService, CartViewModel parentViewModel)
        {
            _cartItem = cartItem;
            _sqlService = sqlService;
            _parentViewModel = parentViewModel;
            Product = product;
            Quantity = cartItem.Quantity;
            UpdateTotalPrice();
        }

        public IRelayCommand DeleteCommand => new RelayCommand(DeleteItem);

        private void DeleteItem()
        {
            _parentViewModel?.RemoveCartItem(this);
        }

        private void UpdateTotalPrice()
        {
            TotalPrice = Product.Price * Quantity;
        }

        [RelayCommand]
        private void IncreaseQuantity()
        {
            if (Quantity < 100)
            {
                Quantity++;
                UpdateCart();
                UpdateTotalPrice();
            }
        }

        [RelayCommand]
        private void DecreaseQuantity()
        {
            if (Quantity > 1)
            {
                Quantity--;
                UpdateCart();
                UpdateTotalPrice();
            }
        }

        partial void OnQuantityChanged(int value)
        {
            if (value < 1)
                Quantity = 1;
            else if (value > 100)
                Quantity = 100;
            
            UpdateCart();
            UpdateTotalPrice();
        }

        private void UpdateCart()
        {
            if (!App.CurrentUserId.HasValue) return;
            _sqlService.UpdateCartItem(App.CurrentUserId.Value, Product.ProductID, Quantity);
        }
    }
}