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
            System.Diagnostics.Debug.WriteLine("ChangePasswordViewModel: Initialized");
        }

        [RelayCommand]
        private async Task Submit()
        {
            System.Diagnostics.Debug.WriteLine("SubmitCommand: Executed");
            // Validate inputs
            if (string.IsNullOrWhiteSpace(CurrentPassword) ||
                string.IsNullOrWhiteSpace(NewPassword) ||
                string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                ErrorMessage = "Vui lòng nhập đầy đủ thông tin.";
                IsErrorVisible = true;
                System.Diagnostics.Debug.WriteLine("SubmitCommand: Validation failed - Empty fields");
                return;
            }

            if (NewPassword != ConfirmPassword)
            {
                ErrorMessage = "Mật khẩu mới và xác nhận mật khẩu không khớp.";
                IsErrorVisible = true;
                System.Diagnostics.Debug.WriteLine("SubmitCommand: Validation failed - Passwords do not match");
                return;
            }

            if (NewPassword.Length < 6)
            {
                ErrorMessage = "Mật khẩu mới phải dài ít nhất 6 ký tự.";
                IsErrorVisible = true;
                System.Diagnostics.Debug.WriteLine("SubmitCommand: Validation failed - Password too short");
                return;
            }

            if (!App.CurrentUserId.HasValue)
            {
                ErrorMessage = "Không tìm thấy người dùng. Vui lòng đăng nhập lại.";
                IsErrorVisible = true;
                System.Diagnostics.Debug.WriteLine("SubmitCommand: Validation failed - No user ID");
                return;
            }

            try
            {
                // Verify current password
                string username = _sqlService.GetUsernameByUserId(App.CurrentUserId.Value);
                System.Diagnostics.Debug.WriteLine($"SubmitCommand: Authenticating with Username={username}, CurrentPassword={CurrentPassword}");
                bool isCurrentPasswordValid = _sqlService.AuthenticateUser(username, CurrentPassword);

                if (!isCurrentPasswordValid)
                {
                    ErrorMessage = "Mật khẩu hiện tại không đúng.";
                    IsErrorVisible = true;
                    System.Diagnostics.Debug.WriteLine("SubmitCommand: Authentication failed");
                    return;
                }

                // Update password in database
                bool isUpdated = _sqlService.UpdateUserPassword(App.CurrentUserId.Value, NewPassword);
                System.Diagnostics.Debug.WriteLine($"SubmitCommand: UpdateUserPassword returned {isUpdated}");
                if (isUpdated)
                {
                    await Application.Current.MainPage.DisplayAlert("Thành công", "Mật khẩu đã được thay đổi.", "OK");
                    await Application.Current.MainPage.Navigation.PopAsync();
                    System.Diagnostics.Debug.WriteLine("SubmitCommand: Password updated and navigated back");
                }
                else
                {
                    ErrorMessage = "Không thể cập nhật mật khẩu. Vui lòng thử lại.";
                    IsErrorVisible = true;
                    System.Diagnostics.Debug.WriteLine("SubmitCommand: Password update failed");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ChangePassword Error: {ex.Message}");
                ErrorMessage = "Đã xảy ra lỗi. Vui lòng thử lại.";
                IsErrorVisible = true;
            }
        }

        [RelayCommand]
        private async Task Cancel()
        {
            System.Diagnostics.Debug.WriteLine("CancelCommand: Executed");
            await Application.Current.MainPage.Navigation.PopAsync();
            System.Diagnostics.Debug.WriteLine("CancelCommand: Navigated back");
        }

    }
}