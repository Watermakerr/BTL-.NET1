using ClothingStoreApp.ViewModels;
using ClothingStoreApp.Services;

namespace ClothingStoreApp.Views
{
    public partial class ChangePasswordPage : ContentPage
    {
        public ChangePasswordPage()
        {
            InitializeComponent();
            var viewModel = App.Services.GetService<ChangePasswordViewModel>();
            if (viewModel == null)
            {
                viewModel = new ChangePasswordViewModel(App.Services.GetService<SqlService>());
                System.Diagnostics.Debug.WriteLine("ChangePasswordPage: ViewModel created manually due to DI failure");
            }
            BindingContext = viewModel;
            System.Diagnostics.Debug.WriteLine("ChangePasswordPage: BindingContext set");
        }
    }
}