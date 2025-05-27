using ClothingStoreApp.Services;
using ClothingStoreApp.ViewModels;

namespace ClothingStoreApp.Views
{
    public partial class CartPage : ContentPage
    {
        private readonly CartViewModel _viewModel;

        public CartPage()
        {
            InitializeComponent();
            _viewModel = new CartViewModel(App.Services.GetService<SqlService>());
            BindingContext = _viewModel;
            System.Diagnostics.Debug.WriteLine("CartPage: Initialized");
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            System.Diagnostics.Debug.WriteLine("CartPage: OnAppearing");
            _viewModel.LoadCartItems();
        }
    }
}