using System.Collections.ObjectModel;
using System.Windows.Input;
using LobasAppOrdersNew.Helpers;
using LobasAppOrdersNew.Models;
using LobasAppOrdersNew.Services;
using LobasAppOrdersNew.Services.Interfaces;

namespace LobasAppOrdersNew.ViewModels
{
    public class CustomerAddressesViewModel : BaseViewModel
    {
        private readonly AddressApiService _addressApiService;

        private int _customerId;
        private string _customerName = string.Empty;
        private string _street = string.Empty;
        private string _city = string.Empty;
        private string _state = string.Empty;
        private string _zipCode = string.Empty;
        private bool _isMain;
        private bool _isSaving;

        public CustomerAddressesViewModel(
            AddressApiService addressApiService,
            IDialogService dialogService,
            INavigationService navigationService) : base(dialogService, navigationService)
        {
            _addressApiService = addressApiService;

            Title = "Direcciones";
            Addresses = new ObservableCollection<AddressModel>();

            SaveAddressCommand = new Command(async () => await SaveAddressAsync());
            CancelCommand = new Command(async () => await NavigationService.GoBackAsync());
        }

        public ObservableCollection<AddressModel> Addresses { get; }

        public int CustomerId
        {
            get => _customerId;
            set => SetProperty(ref _customerId, value);
        }

        public string CustomerName
        {
            get => _customerName;
            set
            {
                if (SetProperty(ref _customerName, value))
                {
                    OnPropertyChanged(nameof(PageSubtitle));
                }
            }
        }

        public string PageSubtitle => string.IsNullOrWhiteSpace(CustomerName)
            ? "Agrega las direcciones del cliente"
            : $"Direcciones de {CustomerName}";

        public string Street
        {
            get => _street;
            set => SetProperty(ref _street, value);
        }

        public string City
        {
            get => _city;
            set => SetProperty(ref _city, value);
        }

        public string State
        {
            get => _state;
            set => SetProperty(ref _state, value);
        }

        public string ZipCode
        {
            get => _zipCode;
            set => SetProperty(ref _zipCode, value);
        }

        public bool IsMain
        {
            get => _isMain;
            set => SetProperty(ref _isMain, value);
        }

        public bool IsSaving
        {
            get => _isSaving;
            set => SetProperty(ref _isSaving, value);
        }

        public ICommand SaveAddressCommand { get; }

        public ICommand CancelCommand { get; }

        public async Task InitializeAsync(int customerId, string customerName)
        {
            CustomerId = customerId;
            CustomerName = customerName;
            await LoadAddressesAsync();
        }

        public async Task LoadAddressesAsync()
        {
            if (IsBusy || CustomerId <= 0)
            {
                return;
            }

            try
            {
                IsBusy = true;
                Addresses.Clear();

                List<AddressModel> addresses =
                    await _addressApiService.GetAddressesByCustomerIdAsync(CustomerId);

                foreach (AddressModel address in addresses)
                {
                    Addresses.Add(address);
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task DeleteAddressAsync(AddressModel address)
        {
            bool confirm = await DialogService.ShowConfirmationAsync(
                "Eliminar direcci\u00f3n",
                $"\u00bfSeguro que deseas eliminar la direcci\u00f3n de {address.Street}?",
                "S\u00ed",
                "Cancelar"
            );

            if (!confirm)
            {
                return;
            }

            bool deleted = await _addressApiService.DeleteAddressAsync(address.Id);

            if (deleted)
            {
                Addresses.Remove(address);
            }
        }

        private async Task SaveAddressAsync()
        {
            if (IsBusy)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(Street) ||
                string.IsNullOrWhiteSpace(City) ||
                string.IsNullOrWhiteSpace(State) ||
                string.IsNullOrWhiteSpace(ZipCode))
            {
                await DialogService.ShowAlertAsync(
                    "Validaci\u00f3n",
                    "Calle, ciudad, estado y c\u00f3digo postal son obligatorios.",
                    "OK"
                );
                return;
            }

            try
            {
                IsBusy = true;
                IsSaving = true;

                AddressRequest request = new AddressRequest
                {
                    CustomerId = CustomerId,
                    Street = Street.Trim(),
                    City = City.Trim(),
                    State = State.Trim(),
                    ZipCode = ZipCode.Trim(),
                    IsMain = IsMain
                };

                bool created = await _addressApiService.CreateAddressAsync(request);

                if (!created)
                {
                    await DialogService.ShowAlertAsync(
                        "Error",
                        "No se pudo guardar la direcci\u00f3n.",
                        "OK"
                    );
                    return;
                }

                ClearForm();
                await ReloadAddressesAsync();
            }
            finally
            {
                IsSaving = false;
                IsBusy = false;
            }
        }

        private void ClearForm()
        {
            Street = string.Empty;
            City = string.Empty;
            State = string.Empty;
            ZipCode = string.Empty;
            IsMain = false;
        }

        private async Task ReloadAddressesAsync()
        {
            Addresses.Clear();

            List<AddressModel> addresses =
                await _addressApiService.GetAddressesByCustomerIdAsync(CustomerId);

            foreach (AddressModel address in addresses)
            {
                Addresses.Add(address);
            }
        }
    }
}
