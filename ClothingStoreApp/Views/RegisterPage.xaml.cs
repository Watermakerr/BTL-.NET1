using ClothingStoreApp.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace ClothingStoreApp.Views;

public partial class RegisterPage : ContentPage
{
    public RegisterPage()
    {
        InitializeComponent();
        BindingContext = App.Services.GetService<RegisterViewModel>();
    }
}