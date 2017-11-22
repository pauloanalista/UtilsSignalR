using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SignalRSelfHosted.Hubs
{
    public class NotificationHub : Hub
    {
        private volatile static List<string> _connectionsIds = new List<string>();



        public override Task OnConnected()
        {
            Console.WriteLine("Connected: " + Context.ConnectionId);
            _connectionsIds.Add(Context.ConnectionId);
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            Console.WriteLine("Disconnected: " + Context.ConnectionId);
            _connectionsIds.Remove(Context.ConnectionId);
            return base.OnDisconnected(stopCalled);
        }

        public bool GetBoolean()
        {
            return true;
        }

        public int GetInt(int i)
        {
            return 1 + i;
        }
        public DateTime GetDateTime()
        {
            return DateTime.Now;
        }
        public string GetDateTimeFormated()
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:sss");

            Clients.All.listenDateTime("Enviado a partir do método [GetDateTimeFormated] -> " + date);

            return date;
        }
        
        public void ListenDateTimeNoParameter()
        {
            Clients.All.listenDateTimeNoParameter();
        }

        public Guid GetGuid()
        {
            return Guid.NewGuid();
        }

        public Pessoa GetPessoa()
        {
            return new Pessoa { Nome = "Roberto Camara" };
        }
    }

    public class Pessoa
    {
        public string Nome { get; set; }
    }
}
