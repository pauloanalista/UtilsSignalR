using log4net;
using Microsoft.AspNet.SignalR.Client;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace SignalRConnection
{

    public sealed class SingletonSignalRConnection : IDisposable
    {
        private static string _hostSignalR;
        private static string _hubName;

        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        // Lock synchronization object
        private static object _instanceLock = new object();
        private static SingletonSignalRConnection _instance = null;
        private static Lazy<HubConnection> _hubConnection = null;
        private static IHubProxy _hubProxy = null;

        private SingletonSignalRConnection()
        {
            try
            {
                _hubConnection = new Lazy<HubConnection>(() => new HubConnection(_hostSignalR));
            }
            catch (Exception ex)
            {
                _logger.Error($"Method: {MethodBase.GetCurrentMethod().Name } - Error: {ex}");
            }
        }

        /// <summary>
        /// Make sure the service is connected
        /// </summary>
        private bool IsConnected
        {
            get
            {
                try
                {
                    if (_hubConnection.Value?.State != ConnectionState.Connected)
                    {
                        _hubProxy = _hubConnection.Value.CreateHubProxy(_hubName);
                        _hubConnection.Value.Start().Wait();
                    }

                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Get instance of the class SingletonSignalRConnection
        /// </summary>
        /// <param name="hostSignalR">The host signalr</param>
        /// <param name="hubName">The name of the hub</param>
        /// <returns>A instance of the class SingletonSignalRConnection</returns>
        public static SingletonSignalRConnection GetInstance(string hostSignalR, string hubName)
        {
            lock (_instanceLock)
            {
                _hostSignalR = hostSignalR;
                _hubName = hubName;

                if (_instance == null)
                    _instance = new SingletonSignalRConnection();
            }

            return _instance;

        }

        /// <summary>
        /// Executes a method on the server side hub asynchronously.
        /// </summary>
        /// <typeparam name="T">The type of result returned from the hub.</typeparam>
        /// <param name="method">The name of the method.</param>
        /// <param name="args">The arguments</param>
        /// <returns>A task that represents when invocation returned.</returns>
        public Task<T> Invoke<T>(string method, params object[] args)
        {
            if (IsConnected)
            {
                return _hubProxy.Invoke<T>(method, args);
            }

            return ReturnDefaultTask<T>();
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
            {
                return _hubProxy.Invoke(method, args);
            }

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
            if (IsConnected)
            {
                return _hubProxy.On<T>(eventName, onData);
            }

            return ReturnDefaultTask<T>();
        }

        /// <summary>
        /// Registers for an event with the specified name and callback
        /// </summary>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="onData">The callback</param>
        /// <returns>An System.IDisposable that represents this subscription.</returns>
        public IDisposable On(string eventName, Action onData)
        {
            if (IsConnected)
            {
                return _hubProxy.On(eventName, onData);
            }

            return default(Task);
        }

        /// <summary>
        /// Get default value or instance of the object
        /// </summary>
        /// <typeparam name="T">The type of result returned from the task</typeparam>
        /// <returns>A task that represents when invocation returned</returns>
        private Task<T> ReturnDefaultTask<T>()
        {
            if (typeof(T).IsValueType || typeof(T) == typeof(string))
                return default(Task<T>);
            else
                return Task.FromResult<T>((T)Activator.CreateInstance(typeof(T)));
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            if (_hubConnection.Value.State == ConnectionState.Connected)
            {
                _hubConnection.Value.Stop();
                _hubConnection.Value.Dispose();
            }
        }

    }
}
