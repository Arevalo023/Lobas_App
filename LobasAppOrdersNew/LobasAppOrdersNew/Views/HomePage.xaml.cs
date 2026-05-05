using LobasAppOrdersNew.ViewModels;

namespace LobasAppOrdersNew.Views
{
    public partial class HomePage : ContentPage
    {
        private readonly HomeViewModel _viewModel;

        public HomePage(HomeViewModel viewModel)
        {
            InitializeComponent();

            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            _viewModel.LoadUserCommand.Execute(null);
        }
    }
}