using LobasAppOrdersNew.Services.Interfaces;

namespace LobasAppOrdersNew.Services
{
    public class DialogService : IDialogService
    {
        public Task ShowAlertAsync(string title, string message, string cancel)
        {
            Page page = GetCurrentPage();

            return page.DisplayAlert(title, message, cancel);
        }

        public Task<bool> ShowConfirmationAsync(
            string title,
            string message,
            string accept,
            string cancel)
        {
            Page page = GetCurrentPage();

            return page.DisplayAlert(title, message, accept, cancel);
        }

        private static Page GetCurrentPage()
        {
            Page? page = Application.Current?.Windows.FirstOrDefault()?.Page;

            return page ?? throw new InvalidOperationException("No active page is available.");
        }
    }
}
