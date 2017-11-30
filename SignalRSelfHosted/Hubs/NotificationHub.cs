using Faker;
using log4net;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SignalRSelfHosted.Hubs
{
    public class NotificationHub : Hub
    {
        private static readonly ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private volatile static List<string> _connectionsIds = new List<string>();

        public override Task OnConnected()
        {
            _logger.Info("Connected NotificationHub: " + Context.ConnectionId);
            _connectionsIds.Add(Context.ConnectionId);
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            _logger.Info("Disconnected NotificationHub: " + Context.ConnectionId);
            _connectionsIds.Remove(Context.ConnectionId);
            return base.OnDisconnected(stopCalled);
        }

        public Task JoinGroup(string groupName)
        {
            return Groups.Add(Context.ConnectionId, groupName);
        }

        public bool GetBoolean()
        {
            return true;
        }

        public void SendInt(int i)
        {
            Clients.All.getInt(1 + i);
            Clients.All.onMethodWithoutReturnValue();

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

        public Guid GetGuid()
        {
            return Guid.NewGuid();
        }

        public void SendNewGuid()
        {
            Clients.All.getNewGuid(Guid.NewGuid());
        }

        public Pessoa GetPessoa()
        {
            return Pessoa.GetPessoaRandom();
        }
    }

    public class Pessoa
    {
        public string Nome { get; set; }
        public string Endereco { get; set; }
        public string Telefone { get; set; }
        public int CartaoCredito { get; set; }

        public static Pessoa GetPessoaRandom()
        {
            return new Pessoa
            {
                Nome = string.Concat(Name.FullName()),
                Endereco = string.Concat(Address.StreetName(), ",", Address.Country(), ",", Address.City()),
                Telefone = Phone.Number(),
                CartaoCredito = RandomNumber.Next(20)
            };
        }
    }
}
