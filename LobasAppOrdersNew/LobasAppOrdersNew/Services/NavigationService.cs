using LobasAppOrdersNew.Services.Interfaces;

namespace LobasAppOrdersNew.Services
{
    public class NavigationService : INavigationService
    {
        public Task GoBackAsync()
        {
            return Shell.Current?.Navigation.NavigationStack.Count > 0
                ? Shell.Current.GoToAsync("..")
                : GetCurrentPage().Navigation.PopAsync();
        }

        public Task GoToRouteAsync(string route)
        {
            return Shell.Current.GoToAsync(route);
        }

        public Task PushAsync(Page page)
        {
            return GetCurrentPage().Navigation.PushAsync(page);
        }

        public Task SetRootAsync(Page page)
        {
            Window? window = Application.Current?.Windows.FirstOrDefault();

            if (window == null)
            {
                throw new InvalidOperationException("No active window is available.");
            }

            window.Page = page;

            return Task.CompletedTask;
        }

        private static Page GetCurrentPage()
        {
            Page? page = Application.Current?.Windows.FirstOrDefault()?.Page;

            return page ?? throw new InvalidOperationException("No active page is available.");
        }
    }
}
