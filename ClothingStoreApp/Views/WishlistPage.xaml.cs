using ClothingStoreApp.ViewModels; // Add this using directive
using ClothingStoreApp.Services; // Add this using directive

namespace ClothingStoreApp.Views;

public partial class WishlistPage : ContentPage
{
    public WishlistPage()
    {
        InitializeComponent();
        System.Diagnostics.Debug.WriteLine($"WishlistPage created for UserID: {App.CurrentUserId}");

        // Set the BindingContext to a new instance of WishlistViewModel
        var sqlService = new SqlService(); // You might want to use DI here instead
        BindingContext = new WishlistViewModel(sqlService);

        System.Diagnostics.Debug.WriteLine("WishlistViewModel BindingContext set");
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        System.Diagnostics.Debug.WriteLine("WishlistPage OnAppearing called");

        if (BindingContext is WishlistViewModel viewModel)
        {
            System.Diagnostics.Debug.WriteLine($"ViewModel found with {viewModel.WishlistProducts.Count} products");
        }
        else
        {
            System.Diagnostics.Debug.WriteLine("BindingContext is not WishlistViewModel or is null");
        }
    }
}