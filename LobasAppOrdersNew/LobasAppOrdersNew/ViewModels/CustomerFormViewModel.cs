using System.Windows.Input;
using LobasAppOrdersNew.Helpers;
using LobasAppOrdersNew.Models;
using LobasAppOrdersNew.Services;

namespace LobasAppOrdersNew.ViewModels
{
    public class CustomerFormViewModel : BaseViewModel
    {
        private readonly CustomerApiService _customerApiService;

        private int _customerId;
        private bool _isEditMode;
        private string _name = string.Empty;
        private string _email = string.Empty;
        private string _phone = string.Empty;
        private bool _isSaving;
        private string _pageHeader = "Agregar cliente";
        private string _pageSubtitle = "Captura la información del cliente";
        private string _saveButtonText = "Guardar cliente";

        public CustomerFormViewModel(CustomerApiService customerApiService)
        {
            _customerApiService = customerApiService;

            Title = "Agregar cliente";

            SaveCustomerCommand = new Microsoft.Maui.Controls.Command(async () => await SaveCustomerAsync());
            CancelCommand = new Microsoft.Maui.Controls.Command(async () => await CancelAsync());
        }

        public int CustomerId
        {
            get => _customerId;
            set => SetProperty(ref _customerId, value);
        }

        public bool IsEditMode
        {
            get => _isEditMode;
            set => SetProperty(ref _isEditMode, value);
        }

        public string PageHeader
        {
            get => _pageHeader;
            set => SetProperty(ref _pageHeader, value);
        }

        public string PageSubtitle
        {
            get => _pageSubtitle;
            set => SetProperty(ref _pageSubtitle, value);
        }

        public string SaveButtonText
        {
            get => _saveButtonText;
            set => SetProperty(ref _saveButtonText, value);
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string Phone
        {
            get => _phone;
            set => SetProperty(ref _phone, value);
        }

        public bool IsSaving
        {
            get => _isSaving;
            set => SetProperty(ref _isSaving, value);
        }

        public ICommand SaveCustomerCommand { get; }

        public ICommand CancelCommand { get; }

        public async Task LoadCustomerForEditAsync(int customerId)
        {
            if (IsBusy)
            {
                return;
            }

            try
            {
                IsBusy = true;

                CustomerModel? customer = await _customerApiService.GetCustomerByIdAsync(customerId);

                if (customer == null)
                {
                    await Application.Current!.MainPage!.DisplayAlert(
                        "Error",
                        "Customer not found.",
                        "OK"
                    );

                    await Shell.Current.GoToAsync("..");
                    return;
                }

                CustomerId = customer.Id;
                IsEditMode = true;

                Title = "Editar cliente";
                PageHeader = "Editar cliente";
                PageSubtitle = "Actualiza la información del cliente";
                SaveButtonText = "Actualizar cliente";

                Name = customer.Name;
                Email = customer.Email ?? string.Empty;
                Phone = customer.Phone ?? string.Empty;
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task SaveCustomerAsync()
        {
            if (IsBusy)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(Name))
            {
                await Application.Current!.MainPage!.DisplayAlert(
                    "Validation",
                    "Customer name is required.",
                    "OK"
                );
                return;
            }

            try
            {
                IsBusy = true;
                IsSaving = true;

                CustomerRequest request = new CustomerRequest
                {
                    Name = Name.Trim(),
                    Email = string.IsNullOrWhiteSpace(Email) ? null : Email.Trim(),
                    Phone = string.IsNullOrWhiteSpace(Phone) ? null : Phone.Trim()
                };

                bool success;

                if (IsEditMode)
                {
                    success = await _customerApiService.UpdateCustomerAsync(CustomerId, request);
                }
                else
                {
                    success = await _customerApiService.CreateCustomerAsync(request);
                }

                if (!success)
                {
                    await Application.Current!.MainPage!.DisplayAlert(
                        "Error",
                        IsEditMode
                            ? "The customer could not be updated."
                            : "The customer could not be created.",
                        "OK"
                    );
                    return;
                }

                await Application.Current!.MainPage!.DisplayAlert(
                    "Success",
                    IsEditMode
                        ? "Customer updated successfully."
                        : "Customer created successfully.",
                    "OK"
                );

                await Shell.Current.GoToAsync("..");
            }
            finally
            {
                IsSaving = false;
                IsBusy = false;
            }
        }

        private async Task CancelAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}