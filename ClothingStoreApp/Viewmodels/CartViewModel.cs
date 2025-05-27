using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using ClothingStoreApp.Models;
using ClothingStoreApp.Services;
using ClothingStoreApp.Views;

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
            System.Diagnostics.Debug.WriteLine("CartViewModel: Initialized");
        }

        public void LoadCartItems()
        {
            CartItems.Clear();
            if (!App.CurrentUserId.HasValue)
            {
                System.Diagnostics.Debug.WriteLine("LoadCartItems: No user logged in, UserID is null");
                return;
            }

            try
            {
                System.Diagnostics.Debug.WriteLine($"LoadCartItems: Fetching items for UserID={App.CurrentUserId.Value}");
                var cartItems = _sqlService.GetCartItems(App.CurrentUserId.Value);
                if (cartItems == null || !cartItems.Any())
                {
                    System.Diagnostics.Debug.WriteLine($"LoadCartItems: No items found for UserID={App.CurrentUserId.Value}");
                    return;
                }

                foreach (var item in cartItems)
                {
                    if (item.Product == null)
                    {
                        System.Diagnostics.Debug.WriteLine($"LoadCartItems: Adding placeholder for UserID={item.Cart.UserID}, ProductID={item.Cart.ProductID} due to missing product");
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
                    System.Diagnostics.Debug.WriteLine($"LoadCartItems: Added item ProductID={item.Cart.ProductID}, Quantity={item.Cart.Quantity}, ProductName={item.Product.ProductName}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadCartItems Error: {ex.Message}");
            }

            UpdateTotalPrice();
            System.Diagnostics.Debug.WriteLine($"LoadCartItems: Loaded {CartItems.Count} items for UserID={App.CurrentUserId.Value}");
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
            System.Diagnostics.Debug.WriteLine($"UpdateTotalPrice: TotalCartPrice={TotalCartPrice:C0}");
        }

        [RelayCommand]
        private async Task PlaceOrder()
        {
            if (!App.CurrentUserId.HasValue)
            {
                System.Diagnostics.Debug.WriteLine("PlaceOrder: No user logged in");
                await Application.Current.MainPage.DisplayAlert("Lỗi", "Vui lòng đăng nhập để đặt hàng.", "OK");
                return;
            }

            if (!CartItems.Any())
            {
                System.Diagnostics.Debug.WriteLine("PlaceOrder: Cart is empty");
                await Application.Current.MainPage.DisplayAlert("Lỗi", "Giỏ hàng trống.", "OK");
                return;
            }

            System.Diagnostics.Debug.WriteLine("PlaceOrder: Order placement triggered");
            await Application.Current.MainPage.DisplayAlert("Thông báo", "Chức năng đặt hàng đang được phát triển.", "OK");
        }

        [RelayCommand]
        private async Task NavigateToHome()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("NavigateToHome: Navigating to HomePage");
                await Application.Current.MainPage.Navigation.PushAsync(new HomePage());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"NavigateToHome Error: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Lỗi", $"Lỗi điều hướng: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task NavigateToWishlist()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("NavigateToWishlist: Navigating to WishlistPage");
                await Application.Current.MainPage.Navigation.PushAsync(new WishlistPage());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"NavigateToWishlist Error: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Lỗi", $"Lỗi điều hướng: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task NavigateToRegister()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("NavigateToRegister: Navigating to RegisterPage");
                await Application.Current.MainPage.Navigation.PushAsync(new RegisterPage());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"NavigateToRegister Error: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Lỗi", $"Lỗi điều hướng: {ex.Message}", "OK");
            }
        }

        public void RemoveCartItem(CartItemViewModel item)
        {
            if (item == null || !App.CurrentUserId.HasValue)
            {
                System.Diagnostics.Debug.WriteLine("RemoveCartItem: Invalid item or no user logged in");
                return;
            }

            try
            {
                bool success = _sqlService.RemoveFromCart(App.CurrentUserId.Value, item.Product.ProductID);
                if (success)
                {
                    CartItems.Remove(item);
                    UpdateTotalPrice();
                    System.Diagnostics.Debug.WriteLine($"RemoveCartItem: Removed ProductID={item.Product.ProductID}, NewCount={CartItems.Count}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"RemoveCartItem: Failed to remove ProductID={item.Product.ProductID}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RemoveCartItem Error: ProductID={item.Product.ProductID}, Error={ex.Message}");
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
            System.Diagnostics.Debug.WriteLine($"CartItemViewModel: Initialized for ProductID={cartItem.ProductID}, Quantity={cartItem.Quantity}");
        }

        public IRelayCommand DeleteCommand => new RelayCommand(DeleteItem);

        private void DeleteItem()
        {
            if (_parentViewModel != null)
            {
                _parentViewModel.RemoveCartItem(this);
                System.Diagnostics.Debug.WriteLine($"DeleteItem: Removed ProductID={Product.ProductID}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"DeleteItem: ParentViewModel is null for ProductID={Product.ProductID}");
            }
        }

        private void UpdateTotalPrice()
        {
            TotalPrice = Product.Price * Quantity;
            System.Diagnostics.Debug.WriteLine($"UpdateTotalPrice: ProductID={Product.ProductID}, Quantity={Quantity}, TotalPrice={TotalPrice:C0}");
        }

        [RelayCommand]
        private void IncreaseQuantity()
        {
            if (Quantity < 100)
            {
                Quantity++;
                UpdateCart();
                UpdateTotalPrice();
                System.Diagnostics.Debug.WriteLine($"IncreaseQuantity: ProductID={Product.ProductID}, Quantity={Quantity}");
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
                System.Diagnostics.Debug.WriteLine($"DecreaseQuantity: ProductID={Product.ProductID}, Quantity={Quantity}");
            }
        }

        partial void OnQuantityChanged(int value)
        {
            if (value < 1)
            {
                Quantity = 1;
                System.Diagnostics.Debug.WriteLine($"OnQuantityChanged: ProductID={Product.ProductID}, Quantity set to minimum 1");
            }
            else if (value > 100)
            {
                Quantity = 100;
                System.Diagnostics.Debug.WriteLine($"OnQuantityChanged: ProductID={Product.ProductID}, Quantity set to maximum 100");
            }
            UpdateCart();
            UpdateTotalPrice();
        }

        private void UpdateCart()
        {
            if (!App.CurrentUserId.HasValue)
            {
                System.Diagnostics.Debug.WriteLine($"UpdateCart: No user logged in for ProductID={Product.ProductID}");
                return;
            }

            try
            {
                bool success = _sqlService.UpdateCartItem(App.CurrentUserId.Value, Product.ProductID, Quantity);
                System.Diagnostics.Debug.WriteLine($"UpdateCart: ProductID={Product.ProductID}, Quantity={Quantity}, Success={success}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UpdateCart Error: ProductID={Product.ProductID}, Error={ex.Message}");
            }
        }
    }
}