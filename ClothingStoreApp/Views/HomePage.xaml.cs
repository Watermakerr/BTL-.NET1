using ClothingStoreApp.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace ClothingStoreApp.Views;

public partial class HomePage : ContentPage
{
    public HomePage()
    {
        InitializeComponent();
        BindingContext = App.Services.GetService<HomeViewModel>();
    }
}
