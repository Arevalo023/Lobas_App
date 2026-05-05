using LobasAppOrdersNew.ViewModels;

namespace LobasAppOrdersNew.Views
{
    public partial class RegisterPage : ContentPage
    {
        public RegisterPage(RegisterViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = viewModel;
        }
    }
}