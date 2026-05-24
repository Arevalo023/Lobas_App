namespace LobasAppOrdersNew.Services.Interfaces
{
    public interface INavigationService
    {
        Task GoBackAsync();

        Task GoToRouteAsync(string route);

        Task PushAsync(Page page);

        Task SetRootAsync(Page page);
    }
}
