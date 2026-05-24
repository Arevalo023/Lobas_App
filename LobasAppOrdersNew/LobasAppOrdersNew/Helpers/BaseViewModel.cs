using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LobasAppOrdersNew.Helpers
{
    using LobasAppOrdersNew.Services.Interfaces;

    public class BaseViewModel : ObservableObject
    {
        private readonly IDialogService? _dialogService;
        private readonly INavigationService? _navigationService;
        private bool _isBusy;
        private string _title = string.Empty;

        protected BaseViewModel()
        {
        }

        protected BaseViewModel(
            IDialogService dialogService,
            INavigationService navigationService)
        {
            _dialogService = dialogService;
            _navigationService = navigationService;
        }

        protected IDialogService DialogService =>
            _dialogService ?? throw new InvalidOperationException("Dialog service was not configured.");

        protected INavigationService NavigationService =>
            _navigationService ?? throw new InvalidOperationException("Navigation service was not configured.");

        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }
    }
}
