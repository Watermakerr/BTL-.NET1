using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ClothingStoreApp.Services;

namespace ClothingStoreApp.ViewModels
{
    public partial class ChangePasswordViewModel : ObservableObject
    {
        private readonly SqlService _sqlService;

        [ObservableProperty]
        private string _currentPassword;

        [ObservableProperty]
        private string _newPassword;

        [ObservableProperty]
        private string _confirmPassword;

        [ObservableProperty]
        private string _errorMessage;

        [ObservableProperty]
        private bool _isErrorVisible;

        public ChangePasswordViewModel(SqlService sqlService)
        {
            _sqlService = sqlService;
        }

        [RelayCommand]
        private async Task Submit()
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(CurrentPassword) ||
                string.IsNullOrWhiteSpace(NewPassword) ||
                string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                ShowError("Vui lòng nhập đầy đủ thông tin.");
                return;
            }

            if (NewPassword != ConfirmPassword)
            {
                ShowError("Mật khẩu mới và xác nhận mật khẩu không khớp.");
                return;
            }

            if (NewPassword.Length < 6)
            {
                ShowError("Mật khẩu mới phải dài ít nhất 6 ký tự.");
                return;
            }

            if (!App.CurrentUserId.HasValue)
            {
                ShowError("Không tìm thấy người dùng. Vui lòng đăng nhập lại.");
                return;
            }

            // Verify current password
            string username = _sqlService.GetUsernameByUserId(App.CurrentUserId.Value);
            bool isCurrentPasswordValid = _sqlService.AuthenticateUser(username, CurrentPassword);

            if (!isCurrentPasswordValid)
            {
                ShowError("Mật khẩu hiện tại không đúng.");
                return;
            }

            // Update password
            bool isUpdated = _sqlService.UpdateUserPassword(App.CurrentUserId.Value, NewPassword);
            if (isUpdated)
            {
                await Application.Current.MainPage.DisplayAlert("Thành công", "Mật khẩu đã được thay đổi.", "OK");
                await Application.Current.MainPage.Navigation.PopAsync();
            }
            else
            {
                ShowError("Không thể cập nhật mật khẩu. Vui lòng thử lại.");
            }
        }

        [RelayCommand]
        private async Task Cancel()
        {
            await Application.Current.MainPage.Navigation.PopAsync();
        }

        private void ShowError(string message)
        {
            ErrorMessage = message;
            IsErrorVisible = true;
        }
    }
}