using log4net;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SignalRSelfHosted.Hubs
{
    public class ExampleHub : Hub
    {
        private static readonly ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private volatile static List<string> _connectionsIds = new List<string>();

        public override Task OnConnected()
        {
            _logger.Info("Connected ExampleHub: " + Context.ConnectionId);
            _connectionsIds.Add(Context.ConnectionId);
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            _logger.Info("Disconnected ExampleHub: " + Context.ConnectionId);
            _connectionsIds.Remove(Context.ConnectionId);
            return base.OnDisconnected(stopCalled);
        }

        public Task JoinGroup(string groupName)
        {
            return Groups.Add(Context.ConnectionId, groupName);
        }

        public Task LeaveGroup(string groupName)
        {
            return Groups.Remove(Context.ConnectionId, groupName);
        }

        public string GetDateTimeFormated()
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:sss");

            Clients.All.listenDateTime("Enviado a partir do método [GetDateTimeFormated] -> " + date);

            return date;
        }
    }
}
