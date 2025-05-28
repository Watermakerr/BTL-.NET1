using ClothingStoreApp.ViewModels;

namespace ClothingStoreApp.Views
{
    public partial class ProfilePage : ContentPage
    {
        public ProfilePage()
        {
            InitializeComponent();
            BindingContext = App.Services.GetService<ProfileViewModel>();
        }
    }
}