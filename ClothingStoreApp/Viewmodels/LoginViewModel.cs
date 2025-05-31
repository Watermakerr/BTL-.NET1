using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ClothingStoreApp.Services;
using ClothingStoreApp.Views;
using System.Threading.Tasks;

namespace ClothingStoreApp.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly SqlService _sqlService;

    [ObservableProperty]
    private string _username;

    [ObservableProperty]
    private string _password;

    [ObservableProperty]
    private string _errorMessage;

    [ObservableProperty]
    private bool _isErrorVisible;

    public LoginViewModel(SqlService sqlService)
    {
        _sqlService = sqlService;
    }

    [RelayCommand]
    private async Task Login(object parameter)
    {
        if (parameter is not ContentPage page)
        {
            ErrorMessage = "Lỗi điều hướng.";
            IsErrorVisible = true;
            return;
        }

        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Vui lòng nhập đầy đủ thông tin.";
            IsErrorVisible = true;
            return;
        }

        bool isAuthenticated = _sqlService.AuthenticateUser(Username, Password);

        if (isAuthenticated)
        {
            int? userId = _sqlService.GetUserIdByUsername(Username);
            if (userId.HasValue)
            {
                App.CurrentUserId = userId.Value;
                IsErrorVisible = false;
                await page.Navigation.PushAsync(new HomePage());
            }
            else
            {
                ErrorMessage = "Không tìm thấy người dùng.";
                IsErrorVisible = true;
            }
        }
        else
        {
            ErrorMessage = "Tên đăng nhập hoặc mật khẩu không đúng.";
            IsErrorVisible = true;
        }
    }

    [RelayCommand]
    private async Task GoToRegister(object parameter)
    {
        if (parameter is not ContentPage page)
        {
            ErrorMessage = "Lỗi điều hướng.";
            IsErrorVisible = true;
            return;
        }

        await page.Navigation.PushAsync(new RegisterPage());
    }
}