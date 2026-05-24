using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;

namespace LobasAppOrdersNew.Services
{
    public class RealtimeNotificationService : IAsyncDisposable
    {
        private readonly HubConnection _connection;
        private readonly SemaphoreSlim _connectionLock = new(1, 1);

        public RealtimeNotificationService(IConfiguration configuration)
        {
            string hubUrl = ApiEndpointResolver.GetNotificationsHubUrl(configuration);

            _connection = new HubConnectionBuilder()
                .WithUrl(hubUrl)
                .WithAutomaticReconnect()
                .Build();

            _connection.On("CustomersChanged", () => CustomersChanged?.Invoke(this, EventArgs.Empty));
            _connection.On("ProductsChanged", () => ProductsChanged?.Invoke(this, EventArgs.Empty));
            _connection.On("OrdersChanged", () => OrdersChanged?.Invoke(this, EventArgs.Empty));
        }

        public event EventHandler? CustomersChanged;

        public event EventHandler? ProductsChanged;

        public event EventHandler? OrdersChanged;

        public async Task StartAsync()
        {
            if (_connection.State != HubConnectionState.Disconnected)
            {
                return;
            }

            await _connectionLock.WaitAsync();

            try
            {
                if (_connection.State == HubConnectionState.Disconnected)
                {
                    await _connection.StartAsync();
                }
            }
            catch
            {
                // The app still works with manual refresh if realtime is temporarily unavailable.
            }
            finally
            {
                _connectionLock.Release();
            }
        }

        public async ValueTask DisposeAsync()
        {
            await _connection.DisposeAsync();
            _connectionLock.Dispose();
        }
    }
}
