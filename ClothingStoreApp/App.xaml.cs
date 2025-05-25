using ClothingStoreApp.Services;
using ClothingStoreApp.ViewModels;
using ClothingStoreApp.Views;
using Microsoft.Extensions.DependencyInjection;

namespace ClothingStoreApp;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; }
    public static int? CurrentUserId { get; set; }

    public App()
    {
        InitializeComponent();

        var services = new ServiceCollection();
        services.AddSingleton<SqlService>();
        services.AddTransient<LoginViewModel>();
        services.AddTransient<RegisterViewModel>();
        services.AddTransient<HomeViewModel>();
        services.AddTransient<ProductDetailViewModel>();
        services.AddTransient<WishlistViewModel>();
        Services = services.BuildServiceProvider();

        MainPage = new NavigationPage(new LoginPage());
    }
}