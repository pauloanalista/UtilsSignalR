using log4net;
using Microsoft.AspNet.SignalR.Client;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace SignalRConnection
{
    public class SignalRCreateConnection : IDisposable
    {

        private Lazy<HubConnection> _hubConnection = null;
        private IHubProxy _hubProxy = null;

        private string _hostSignalR;
        private string _hubName;

        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public bool ServerOnline { get; private set; }

        public string ConnectionId => _hubConnection.Value.ConnectionId;

        public SignalRCreateConnection(string hostSignalR, string hubName)
        {
            _hostSignalR = hostSignalR;
            _hubName = hubName;
            _hubConnection = new Lazy<HubConnection>(() => new HubConnection(hostSignalR));
            _hubProxy = _hubConnection.Value.CreateHubProxy(hubName);

            _hubConnection.Value.Closed += HubConnection_Closed;

            ServerOnline = true;
        }

        /// <summary>
        /// Occurs when the Microsoft.AspNet.SignalR.Client.Connection is stopped.
        /// </summary>
        private void HubConnection_Closed()
        {
            ServerOnline = false;//executar alguma ação quando conexão estiver fechada
            _logger.Info($"Method: {MethodBase.GetCurrentMethod().Name } -> Servidor Offline");
        }

        private async Task StartHubConnectionAsync()
        {
            if (_hubConnection.Value.State != ConnectionState.Connected)
            {
                try
                {
                    await _hubConnection.Value.Start();
                    _logger.Info($"Method: {MethodBase.GetCurrentMethod().Name } -> Connected in Host: {_hostSignalR} -> Hub: {_hubName}");
                }
                catch (Exception ex)
                {
                    _logger.Error($"Method: {MethodBase.GetCurrentMethod().Name } - Error: {ex}");
                }
            }

            if (_hubConnection.Value.State != ConnectionState.Disconnected)
                ServerOnline = true;
        }

        /// <summary>
        /// Make sure the service is connected
        /// </summary>
        private bool IsConnected
        {
            get
            {
                bool isConnected = (_hubConnection.Value.State == ConnectionState.Connected);

                if (!isConnected)
                    StartHubConnectionAsync().Wait();

                return _hubConnection.Value.State == ConnectionState.Connected;
            }
        }

        /// <summary>
        /// Get default value or instance of the object
        /// </summary>
        /// <typeparam name="T">The type of result returned from the task</typeparam>
        /// <returns>A task that represents when invocation returned</returns>
        private Task<T> ReturnDefaultTask<T>()
        {
            if (typeof(T).IsValueType || typeof(T) == typeof(string))
            {
                T value = default(T);
                return Task.FromResult<T>(value);
            }
            else
                return Task.FromResult<T>((T)Activator.CreateInstance(typeof(T)));
        }

        /// <summary>
        /// Executes a method on the server side hub asynchronously.
        /// </summary>
        /// <typeparam name="T">The type of result returned from the hub.</typeparam>
        /// <param name="method">The name of the method.</param>
        /// <param name="args">The arguments</param>
        /// <returns>A task that represents when invocation returned.</returns>
        public async Task<T> Invoke<T>(string method, params object[] args)
        {
            if (IsConnected)
                return await _hubProxy.Invoke<T>(method, args);

            return await ReturnDefaultTask<T>();
        }

        /// <summary>
        /// Executes a method on the server side hub asynchronously.
        /// </summary>
        /// <param name="method">The name of the method.</param>
        /// <param name="args">The arguments</param>
        /// <returns>A task that represents when invocation returned.</returns>
        public Task Invoke(string method, params object[] args)
        {
            if (IsConnected)
                return _hubProxy.Invoke(method, args);


            return default(Task);
        }

        /// <summary>
        /// Registers for an event with the specified name and callback
        /// </summary>
        /// <typeparam name="T">The type of result returned from the hub.</typeparam>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="onData">The callback</param>
        /// <returns>An System.IDisposable that represents this subscription.</returns>
        public IDisposable On<T>(string eventName, Action<T> onData)
        {
            StartHubConnectionAsync().Wait();

            var result = _hubProxy.On<T>(eventName, onData);

            return result;
        }

        
        public IDisposable On<T1, T2>(string eventName, Action<T1, T2> onData)
        {
            StartHubConnectionAsync().Wait();

            var result = _hubProxy.On<T1, T2>(eventName, onData);

            return result;
        }

        public IDisposable On<T1, T2, T3>(string eventName, Action<T1, T2, T3> onData)
        {
            StartHubConnectionAsync().Wait();

            var result = _hubProxy.On<T1, T2, T3>(eventName, onData);

            return result;
        }

        public IDisposable On<T1, T2, T3, T4>(string eventName, Action<T1, T2, T3, T4> onData)
        {
            StartHubConnectionAsync().Wait();

            var result = _hubProxy.On<T1, T2, T3, T4>(eventName, onData);

            return result;
        }

        public IDisposable On<T1, T2, T3, T4, T5>(string eventName, Action<T1, T2, T3, T4, T5> onData)
        {
            StartHubConnectionAsync().Wait();

            var result = _hubProxy.On<T1, T2, T3, T4, T5>(eventName, onData);

            return result;
        }

        public IDisposable On<T1, T2, T3, T4, T5, T6>(string eventName, Action<T1, T2, T3, T4, T5, T6> onData)
        {
            StartHubConnectionAsync().Wait();

            var result = _hubProxy.On<T1, T2, T3, T4, T5, T6>(eventName, onData);

            return result;
        }

        public IDisposable On<T1, T2, T3, T4, T5, T6, T7>(string eventName, Action<T1, T2, T3, T4, T5, T6, T7> onData)
        {
            StartHubConnectionAsync().Wait();

            var result = _hubProxy.On<T1, T2, T3, T4, T5, T6, T7>(eventName, onData);

            return result;
        }

        /// <summary>
        /// Registers for an event with the specified name and callback
        /// </summary>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="onData">The callback</param>
        /// <returns>An System.IDisposable that represents this subscription.</returns>
        public IDisposable On(string eventName, Action onData)
        {
            StartHubConnectionAsync().Wait();

            var result = _hubProxy.On(eventName, onData);

            return result;
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            _hubConnection.Value.Stop();
            _hubConnection.Value.Dispose();

        }

    }


}
