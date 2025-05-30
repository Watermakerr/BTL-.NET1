using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ClothingStoreApp.Services;
using ClothingStoreApp.Views;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using System;

namespace ClothingStoreApp.ViewModels;

public partial class RegisterViewModel : ObservableObject
{
    private readonly SqlService _sqlService;

    [ObservableProperty]
    private string _username;

    [ObservableProperty]
    private string _password;

    [ObservableProperty]
    private string _phoneNumber;

    [ObservableProperty]
    private string _address;

    [ObservableProperty]
    private string _message;

    [ObservableProperty]
    private bool _isMessageVisible;

    [ObservableProperty]
    private Color _messageColor;

    public RegisterViewModel(SqlService sqlService)
    {
        _sqlService = sqlService;
    }

    [RelayCommand]
    private async Task Register(object parameter)
    {
        if (parameter is not ContentPage page)
        {
            Message = "Lỗi điều hướng.";
            MessageColor = Colors.Red;
            IsMessageVisible = true;
            return;
        }

        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password) ||
            string.IsNullOrWhiteSpace(PhoneNumber) || string.IsNullOrWhiteSpace(Address))
        {
            Message = "Vui lòng nhập đầy đủ thông tin.";
            MessageColor = Colors.Red;
            IsMessageVisible = true;
            return;
        }

        bool isRegistered = _sqlService.RegisterUser(Username, Password, PhoneNumber, Address);

        if (isRegistered)
        {
            Message = "Đăng ký thành công! Quay lại đăng nhập.";
            MessageColor = Colors.Green;
            IsMessageVisible = true;
            await page.Navigation.PushAsync(new LoginPage());
        }
        else
        {
            Message = "Tên đăng nhập đã tồn tại.";
            MessageColor = Colors.Red;
            IsMessageVisible = true;
        }
    }
}