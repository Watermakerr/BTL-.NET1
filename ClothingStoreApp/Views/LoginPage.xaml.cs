using ClothingStoreApp.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace ClothingStoreApp.Views;

public partial class LoginPage : ContentPage

{
    public LoginPage()
    {
        InitializeComponent();
        BindingContext = App.Services.GetService<LoginViewModel>();
    }
}