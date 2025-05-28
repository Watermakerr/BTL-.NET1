using ClothingStoreApp.ViewModels;

namespace ClothingStoreApp.Views
{
    public partial class ConfirmationModalPage : ContentPage
    {
        private readonly CartViewModel _cartViewModel;

        public ConfirmationModalPage(CartViewModel cartViewModel)
        {
            InitializeComponent();
            _cartViewModel = cartViewModel;
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
            System.Diagnostics.Debug.WriteLine("ConfirmationModalPage: Cancelled");
        }

        private async void OnConfirmClicked(object sender, EventArgs e)
        {
            string address = AddressEntry.Text;
            if (string.IsNullOrWhiteSpace(address))
            {
                await DisplayAlert("Lỗi", "Vui lòng nhập địa chỉ giao hàng.", "OK");
                return;
            }

            await _cartViewModel.ConfirmOrderAsync(address);
            await Navigation.PopModalAsync();
            System.Diagnostics.Debug.WriteLine($"ConfirmationModalPage: Order confirmed with address: {address}");
        }
    }
}