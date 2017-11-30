# UtilsSignalR

### SignalRConnection
- Encapsula a criação de conexões e chamada aos métodos de um serviço SignalR.
  
### Exemplo 
- SignalRCreateConnection
```csharp
    class Pessoa
    {
        public string Nome { get; set; }
        public string Endereco { get; set; }
        public string Telefone { get; set; }
        public int CartaoCredito { get; set; }
    }

    class Program
    {
        private static string host = "http://localhost:8082/signalr";

        static void Main()
        {
            Task.Factory.StartNew(() => OnExempleHub());
            Task.Factory.StartNew(() => InvokeExempleHub());
            Task.Factory.StartNew(() => InvokeNotificationHub());
            Console.ReadKey();
        }

        private static void OnExempleHub()
        {

            Task.Factory.StartNew(() =>
            {
                SignalRCreateConnection notificationHub = new SignalRCreateConnection(host, "ExampleHub");
                notificationHub.On<string>("listenDateTime", d =>
                {
                    Console.WriteLine(d);
                });

                notificationHub.On<string>("getNewGuid", g =>
                {
                    Console.WriteLine(g);
                });

                do
                {
                    //executar alguma ação caso o serviço esteja indisponível
                    if (!notificationHub.ServerOnline)
                    {
                        Console.WriteLine("Server SignalR OffLine.");
                    }

                    Thread.Sleep(2000);
                } while (true);

            });

            Console.ReadKey();

        }

        private static void InvokeNotificationHub()
        {
            SignalRCreateConnection notificationHub = new SignalRCreateConnection(host, "NotificationHub");

            while (true)
            {
                notificationHub.Invoke("SendNewGuid");
                Task<Pessoa> pessoa = notificationHub.Invoke<Pessoa>("GetPessoa");

                if (!notificationHub.ServerOnline)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Server OffLine -> {(string.IsNullOrEmpty(pessoa.Result.Nome) ? "No Connection - Result [GetPessoa]: Null" : "Connected - Result [GetPessoa]: " + pessoa.Result.Nome)}");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine($"Server OnLine -> {(string.IsNullOrEmpty(pessoa.Result.Nome) ? "No Connection - Result [GetPessoa]: Null" : "Connected - Result [GetPessoa]: " + pessoa.Result.Nome)}");
                }

                Thread.Sleep(1000);
            }
        }

        private static void InvokeExempleHub()
        {
            SignalRCreateConnection exemploHub = new SignalRCreateConnection(host, "ExampleHub");

            while (true)
            {
                Task<string> date = exemploHub.Invoke<string>("GetDateTimeFormated");

                if (!exemploHub.ServerOnline)
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Server OffLine -> {(string.IsNullOrEmpty(date.Result) ? "No Connection - Result [GetDateTimeFormated]: Null" : "Connected - Result [GetDateTimeFormated]: " + date.Result)}");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine($"Server OnLine -> {(string.IsNullOrEmpty(date.Result) ? "No Connection - Result [GetDateTimeFormated]: Null" : "Connected - Result [GetDateTimeFormated]: " + date.Result)}");
                }

                Thread.Sleep(1000);
            }
        }
    }
    
    
```
  
### Referências
- SignalR (https://www.asp.net/signalr)
- Topshelf (http://topshelf-project.com/index.html)
- Log4net (https://logging.apache.org/log4net/)
- SimpleInjector (https://simpleinjector.readthedocs.io/en/latest/webapiintegration.html)
